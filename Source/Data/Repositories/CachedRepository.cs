using Common.Services;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class CachedRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly ICacheService _cacheService;
    protected readonly string _cacheKeyPrefix;

    protected CachedRepository(IRepository<TEntity> repository, ICacheService cacheService, string cacheKeyPrefix)
    {
        _repository = repository;
        _cacheService = cacheService;
        _cacheKeyPrefix = cacheKeyPrefix;
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{_cacheKeyPrefix}:id:{id}";
        var cachedEntity = await _cacheService.GetAsync<TEntity>(cacheKey, cancellationToken);
        
        if (cachedEntity != null)
            return cachedEntity;

        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await _cacheService.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(15), cancellationToken);
        }

        return entity;
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{_cacheKeyPrefix}:all";
        var cachedEntities = await _cacheService.GetAsync<List<TEntity>>(cacheKey, cancellationToken);
        
        if (cachedEntities != null)
            return cachedEntities;

        var entities = await _repository.GetAllAsync(cancellationToken);
        await _cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(10), cancellationToken);

        return entities;
    }

    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For complex queries, we don't cache due to the difficulty of generating cache keys
        return await _repository.FindAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _repository.AddAsync(entity, cancellationToken);
        await InvalidateCacheAsync(cancellationToken);
        return result;
    }

    public virtual async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var result = await _repository.AddRangeAsync(entities, cancellationToken);
        await InvalidateCacheAsync(cancellationToken);
        return result;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _repository.UpdateAsync(entity, cancellationToken);
        await InvalidateCacheAsync(cancellationToken);
        return result;
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(entity, cancellationToken);
        await InvalidateCacheAsync(cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteRangeAsync(entities, cancellationToken);
        await InvalidateCacheAsync(cancellationToken);
    }

    protected virtual async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheService.RemovePatternAsync($"{_cacheKeyPrefix}:*", cancellationToken);
    }
}