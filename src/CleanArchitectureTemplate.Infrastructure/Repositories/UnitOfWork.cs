using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using CleanArchitectureTemplate.Infrastructure.Context;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public sealed class UnitOfWork(
    AppDbContext context,
    IProductRepository products,
    IRefreshTokenRepository refreshTokens) : IUnitOfWork
{
    public IProductRepository Products { get; } = products;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
