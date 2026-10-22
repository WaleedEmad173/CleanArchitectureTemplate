using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using RefreshTokenEntity = CleanArchitectureTemplate.Domain.Entities.RefreshToken;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var hash = tokenGenerator.HashToken(request.Request.RefreshToken);

        var stored = await unitOfWork.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (stored is null || !stored.IsActive)
            throw new UnauthorizedException("Refresh token is invalid or expired.");

        var now = DateTime.UtcNow;
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (!await unitOfWork.RefreshTokens.TryRevokeActiveAsync(stored.Id, stored.UserId, now, cancellationToken))
                throw new UnauthorizedException("Refresh token is invalid or expired.");

            var user = await identityService.GetByIdAsync(
                stored.UserId, cancellationToken);

            if (user is null)
                throw new UnauthorizedException("User no longer exists.");

            var roles = await identityService.GetRolesAsync(user.Id, cancellationToken);

            var access = await jwtService.CreateAccessTokenAsync(
                user.Id, user.Email, user.UserName, user.FullName, roles, cancellationToken);

            var rawRefreshToken = tokenGenerator.GenerateRefreshToken();

            var replacement = new RefreshTokenEntity
            {
                UserId = user.Id,
                TokenHash = tokenGenerator.HashToken(rawRefreshToken),
                ExpiresAtUtc = now.AddDays(14)
            };

            await unitOfWork.RefreshTokens
                .AddAsync(replacement, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return new AuthResponseDto(
                access.AccessToken,
                access.ExpiresAtUtc,
                rawRefreshToken,
                replacement.ExpiresAtUtc);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
