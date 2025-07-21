using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Repositories;

[Collection(DomainTestCategories.Repository)]
public class IApplicationUserRepositoryTests {
    [Fact]
    public void ApplicationUserRepository_Should_Be_Created_Successfully() {
        // Arrange & Act
        var repository = Substitute.For<IApplicationUserRepository>();

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IApplicationUserRepository>();
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_GetByIdAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var userId = "user-123";
        var expectedUser = new ApplicationUser("testuser", "test@example.com");
        repository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act
        var result = await repository.GetByIdAsync(userId);

        // Assert
        result.Should().Be(expectedUser);
        await repository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_GetByUserNameAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var userName = "testuser";
        var expectedUser = new ApplicationUser(userName, "test@example.com");
        repository.GetByUserNameAsync(userName, Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act
        var result = await repository.GetByUserNameAsync(userName);

        // Assert
        result.Should().Be(expectedUser);
        await repository.Received(1).GetByUserNameAsync(userName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_GetByEmailAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var email = "test@example.com";
        var expectedUser = new ApplicationUser("testuser", email);
        repository.GetByEmailAsync(email, Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act
        var result = await repository.GetByEmailAsync(email);

        // Assert
        result.Should().Be(expectedUser);
        await repository.Received(1).GetByEmailAsync(email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_IsUserNameAvailableAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var userName = "newuser";
        repository.IsUserNameAvailableAsync(userName, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await repository.IsUserNameAvailableAsync(userName);

        // Assert
        result.Should().BeTrue();
        await repository.Received(1).IsUserNameAvailableAsync(userName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_IsEmailAvailableAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var email = "new@example.com";
        repository.IsEmailAvailableAsync(email, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await repository.IsEmailAvailableAsync(email);

        // Assert
        result.Should().BeTrue();
        await repository.Received(1).IsEmailAvailableAsync(email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_GetPublishersAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var publisher1 = new ApplicationUser("publisher1", "pub1@example.com");
        publisher1.EnablePublisher();
        var publisher2 = new ApplicationUser("publisher2", "pub2@example.com");
        publisher2.EnablePublisher();
        
        var expectedPublishers = new List<ApplicationUser> { publisher1, publisher2 };
        repository.GetPublishersAsync(Arg.Any<CancellationToken>()).Returns(expectedPublishers);

        // Act
        var result = await repository.GetPublishersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(publisher1);
        result.Should().Contain(publisher2);
        await repository.Received(1).GetPublishersAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_SearchAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var searchTerm = "test";
        var user1 = new ApplicationUser("testuser", "test@example.com");
        var user2 = new ApplicationUser("anothertest", "another@test.com");
        
        var expectedResults = new List<ApplicationUser> { user1, user2 };
        repository.SearchAsync(searchTerm, Arg.Any<CancellationToken>()).Returns(expectedResults);

        // Act
        var result = await repository.SearchAsync(searchTerm);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(user1);
        result.Should().Contain(user2);
        await repository.Received(1).SearchAsync(searchTerm, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplicationUserRepository_Should_Handle_AddAsync() {
        // Arrange
        var repository = Substitute.For<IApplicationUserRepository>();
        var user = new ApplicationUser("testuser", "test@example.com");
        repository.AddAsync(user, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        result.Should().Be(user);
        await repository.Received(1).AddAsync(user, Arg.Any<CancellationToken>());
    }
}