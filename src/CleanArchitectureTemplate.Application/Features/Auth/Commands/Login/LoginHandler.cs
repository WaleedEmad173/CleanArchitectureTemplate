using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using RefreshTokenEntity = CleanArchitectureTemplate.Domain.Entities.RefreshToken;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Login;

public sealed class LoginHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await identityService.GetByEmailAsync(
            request.Request.Email, cancellationToken);

        if (user is null ||
            !await identityService.CheckPasswordAsync(
                user.Id, request.Request.Password, cancellationToken))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var roles = await identityService.GetRolesAsync(user.Id, cancellationToken);

        var access = await jwtService.CreateAccessTokenAsync(
            user.Id, user.Email, user.UserName, user.FullName, roles, cancellationToken);

        var rawRefreshToken = tokenGenerator.GenerateRefreshToken();

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = tokenGenerator.HashToken(rawRefreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
        };

        await unitOfWork.RefreshTokens
            .AddAsync(refreshToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            access.AccessToken,
            access.ExpiresAtUtc,
            rawRefreshToken,
            refreshToken.ExpiresAtUtc);
    }
}
