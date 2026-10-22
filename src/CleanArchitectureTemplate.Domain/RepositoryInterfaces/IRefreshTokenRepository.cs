using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Domain.RepositoryInterfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<bool> TryRevokeActiveAsync(int id, int updatedBy, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
        Task<int> RevokeAllActiveForUserAsync(int userId, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
    }
}
