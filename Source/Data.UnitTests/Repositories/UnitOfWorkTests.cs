using Common.UnitTests.TestUtilities;

using Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.UnitTests.Repositories;

[TestCategory(TestCategories.Integration)]
public class UnitOfWorkTests : DatabaseTestBase {
    private McpHubContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;

    protected override void ConfigureDatabaseServices() {
        base.ConfigureDatabaseServices();

        Services.AddDbContext<McpHubContext>(options =>
            options.UseInMemoryDatabase(DatabaseName));

        Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    [SetUp]
    public void SetUp() {
        _context = CreateInMemoryContext<McpHubContext>();
        _unitOfWork = GetService<IUnitOfWork>();
    }

    [Test]
    public async Task SaveChangesAsync_WhenChangesExist_SavesChangesToDatabase() {
        // Arrange
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        await _unitOfWork.Publishers.AddAsync(publisher);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1); // One entity was saved

        var savedPublisher = await _unitOfWork.Publishers.GetByIdAsync(publisher.Id);
        savedPublisher.Should().NotBeNull();
        savedPublisher!.Name.Should().Be("Test Publisher");
    }

    [Test]
    public async Task BeginTransactionAsync_CommitTransactionAsync_SavesChangesWithinTransaction() {
        // Arrange
        var publisher = new Publisher("Transaction Publisher", "transaction@example.com", PublisherType.Individual);

        // Act
        await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedPublisher = await _unitOfWork.Publishers.GetByIdAsync(publisher.Id);
        savedPublisher.Should().NotBeNull();
        savedPublisher!.Name.Should().Be("Transaction Publisher");
    }

    [Test]
    public async Task BeginTransactionAsync_RollbackTransactionAsync_RevertsChanges() {
        // Arrange
        var publisher = new Publisher("Rollback Publisher", "rollback@example.com", PublisherType.Individual);

        // Act
        await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        var savedPublisher = await _unitOfWork.Publishers.GetByIdAsync(publisher.Id);
        savedPublisher.Should().BeNull();
    }

    [Test]
    public async Task MultipleRepositories_WorkWithSameContext() {
        // Arrange
        var publisher = new Publisher("Multi Repo Publisher", "multi@example.com", PublisherType.Individual);
        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        var server = new Server("Test Server", "A test server", publisher.Id);
        await _unitOfWork.Servers.AddAsync(server);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var savedPublisher = await _unitOfWork.Publishers.GetByIdAsync(publisher.Id);
        var savedServer = await _unitOfWork.Servers.GetByIdAsync(server.Id);

        // Assert
        savedPublisher.Should().NotBeNull();
        savedServer.Should().NotBeNull();
        savedServer!.PublisherId.Should().Be(publisher.Id);
    }

    [Test]
    public async Task TransactionScope_RollbackOnException_RevertsAllChanges() {
        // Arrange
        var publisher1 = new Publisher("Publisher 1", "pub1@example.com", PublisherType.Individual);
        var publisher2 = new Publisher("Publisher 2", "pub2@example.com", PublisherType.Individual);

        // Act & Assert
        await _unitOfWork.BeginTransactionAsync();

        try {
            await _unitOfWork.Publishers.AddAsync(publisher1);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.Publishers.AddAsync(publisher2);
            await _unitOfWork.SaveChangesAsync();

            // Simulate an exception
            throw new InvalidOperationException("Test exception");
        }
        catch (InvalidOperationException) {
            await _unitOfWork.RollbackTransactionAsync();
        }

        // Assert - both publishers should not be saved
        var savedPublisher1 = await _unitOfWork.Publishers.GetByIdAsync(publisher1.Id);
        var savedPublisher2 = await _unitOfWork.Publishers.GetByIdAsync(publisher2.Id);

        savedPublisher1.Should().BeNull();
        savedPublisher2.Should().BeNull();
    }

    [Test]
    public void Repositories_AreNotNull() {
        // Assert
        _unitOfWork.Publishers.Should().NotBeNull();
        _unitOfWork.Servers.Should().NotBeNull();
        _unitOfWork.ServerVersions.Should().NotBeNull();
        _unitOfWork.Packages.Should().NotBeNull();
        _unitOfWork.PackageVersions.Should().NotBeNull();
        _unitOfWork.SecurityScans.Should().NotBeNull();
    }

    [TearDown]
    public async Task TearDown() => await CleanupDatabaseAsync();

    protected override async Task CleanupDatabaseAsync() {
        if (_context != null) {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        _unitOfWork?.Dispose();
    }
}