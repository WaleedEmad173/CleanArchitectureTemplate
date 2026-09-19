using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity>();

    public IQueryable<TEntity> Query() => _set;

    public Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _set.AnyAsync(predicate, cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _set.CountAsync(cancellationToken)
            : _set.CountAsync(predicate, cancellationToken);

    public async Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _set.CountAsync(cancellationToken);

        var items = await _set
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default) =>
        _set.AddAsync(entity, cancellationToken).AsTask();

    public void AddRange(IEnumerable<TEntity> entities) =>
        _set.AddRange(entities);

    public void Update(TEntity entity) =>
        _set.Update(entity);

    public void Remove(TEntity entity) =>
        _set.Remove(entity);
}
