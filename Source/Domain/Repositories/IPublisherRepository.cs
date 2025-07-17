using Domain.Entities;

namespace Domain.Repositories;

public interface IPublisherRepository : IRepository<Publisher> {
    Task<Publisher?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<Publisher>> GetVerifiedPublishersAsync(CancellationToken cancellationToken = default);

    Task<List<Publisher>> GetPublishersByTypeAsync(PublisherType type, CancellationToken cancellationToken = default);

    Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default);

    Task<Publisher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}