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
        int updatedBy,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.RefreshTokens
            .Where(token => token.Id == id && token.RevokedAtUtc == null && token.ExpiresAtUtc > revokedAtUtc)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(token => token.RevokedAtUtc, revokedAtUtc)
                    .SetProperty(token => token.UpdatedAtUtc, revokedAtUtc)
                    .SetProperty(token => token.UpdatedBy, (int?)updatedBy),
                cancellationToken);

        return affected == 1;
    }

    public Task<int> RevokeAllActiveForUserAsync(
        int userId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default) =>
        context.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAtUtc == null && token.ExpiresAtUtc > revokedAtUtc)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(token => token.RevokedAtUtc, revokedAtUtc)
                    .SetProperty(token => token.UpdatedAtUtc, revokedAtUtc)
                    .SetProperty(token => token.UpdatedBy, userId),
                cancellationToken);
}
