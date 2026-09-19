using CleanArchitectureTemplate.Domain.Entities;
using System.Linq.Expressions;

namespace CleanArchitectureTemplate.Domain.RepositoryInterfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
