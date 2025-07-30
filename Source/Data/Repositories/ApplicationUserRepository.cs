using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

public class ApplicationUserRepository(McpHubContext context) : IApplicationUserRepository {
    private readonly McpHubContext _context = context;
    private readonly DbSet<ApplicationUser> _dbSet = context.Set<ApplicationUser>();

    public virtual async Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

    public virtual async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public virtual async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<ApplicationUser?> FirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(predicate, cancellationToken);

    public virtual async Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate == null ? await _dbSet.CountAsync(cancellationToken) : await _dbSet.CountAsync(predicate, cancellationToken);

    public virtual async Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default) {
        await _dbSet.AddAsync(user, cancellationToken);
        return user;
    }

    public virtual Task<ApplicationUser> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default) {
        _dbSet.Update(user);
        return Task.FromResult(user);
    }

    public virtual Task DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default) {
        _dbSet.Remove(user);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> IsUserNameAvailableAsync(string userName, CancellationToken cancellationToken = default)
        => !await _dbSet.AnyAsync(u => u.UserName == userName, cancellationToken);

    public virtual async Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default)
        => !await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);

    public virtual async Task<List<ApplicationUser>> GetPublishersAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(u => u.IsPublisher).ToListAsync(cancellationToken);

    public virtual async Task<List<ApplicationUser>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) {
        var normalizedSearchTerm = searchTerm.ToUpperInvariant();
        return await _dbSet.Where(u =>
            u.UserName!.ToUpper().Contains(normalizedSearchTerm) ||
            (u.DisplayName != null && u.DisplayName.ToUpper().Contains(normalizedSearchTerm)) ||
            (u.Email != null && u.Email.ToUpper().Contains(normalizedSearchTerm)) ||
            (u.GitHubUsername != null && u.GitHubUsername.ToUpper().Contains(normalizedSearchTerm))
        ).ToListAsync(cancellationToken);
    }
}