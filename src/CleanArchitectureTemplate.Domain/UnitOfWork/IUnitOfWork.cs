using CleanArchitectureTemplate.Domain.RepositoryInterfaces;

namespace CleanArchitectureTemplate.Domain.UnitOfWork;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
