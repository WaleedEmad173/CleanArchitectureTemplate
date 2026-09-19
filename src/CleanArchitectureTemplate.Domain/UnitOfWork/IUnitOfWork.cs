using CleanArchitectureTemplate.Domain.RepositoryInterfaces;

namespace CleanArchitectureTemplate.Domain.UnitOfWork;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
