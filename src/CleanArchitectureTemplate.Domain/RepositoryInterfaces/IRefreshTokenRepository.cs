using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Domain.RepositoryInterfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<bool> TryRevokeActiveAsync(int id, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
    }
}
