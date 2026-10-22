using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using CleanArchitectureTemplate.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<bool> TryRevokeActiveAsync(
        int id,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.RefreshTokens
            .Where(token => token.Id == id && token.RevokedAtUtc == null && token.ExpiresAtUtc > revokedAtUtc)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(token => token.RevokedAtUtc, revokedAtUtc),
                cancellationToken);

        return affected == 1;
    }
}
