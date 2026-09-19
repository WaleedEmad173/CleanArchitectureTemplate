using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using CleanArchitectureTemplate.Infrastructure.Context;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
}
