using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using CleanArchitectureTemplate.Infrastructure.Context;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IProductRepository? _products;
    private IRefreshTokenRepository? _refreshTokens;

    public IProductRepository Products => _products ??= new ProductRepository(context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
