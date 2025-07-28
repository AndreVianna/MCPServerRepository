namespace MCPHub.Domain.Repositories;

/// <summary>
/// Interface for testing purposes.
/// </summary>
public interface ITestServerRepository {
    Task<object?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
