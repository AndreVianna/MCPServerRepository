using System.Linq.Expressions;

using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Repositories;

/// <summary>
/// Repository interface for ApplicationUser entity
/// Note: Uses string-based IDs due to IdentityUser inheritance
/// </summary>
public interface IApplicationUserRepository {
    Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> FirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    Task<ApplicationUser> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    Task DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    Task<bool> IsUserNameAvailableAsync(string userName, CancellationToken cancellationToken = default);

    Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);

    Task<List<ApplicationUser>> GetPublishersAsync(CancellationToken cancellationToken = default);

    Task<List<ApplicationUser>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}