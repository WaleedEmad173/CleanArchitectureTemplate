using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using Google.Apis.Auth;
using MediatR;
using Microsoft.Extensions.Configuration;
using RefreshTokenEntity = CleanArchitectureTemplate.Domain.Entities.RefreshToken;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin
{
    public sealed class GoogleLoginHandler(
        IConfiguration configuration,
        IIdentityService identityService,
        IJwtService jwtService,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
        : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var clientId = configuration["GoogleAuth:ClientId"];
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.Request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
            }
            catch
            {
                throw new BadRequestException("Invalid Google token.");
            }

            var user = await identityService.GetByEmailAsync(payload.Email, cancellationToken);

            if (user == null)
            {
                var randomPassword = Guid.NewGuid().ToString() + "Aa1@";

                var result = await identityService.CreateAsync(
                    payload.Name,
                    payload.Email,
                    payload.Email,
                    randomPassword,
                    cancellationToken);

                if (!result.Succeeded)
                    throw new BadRequestException("Failed to create user from Google account.");

                user = await identityService.GetByEmailAsync(payload.Email, cancellationToken);
            }

            var roles = await identityService.GetRolesAsync(user!.Id, cancellationToken);
            var access = await jwtService.CreateAccessTokenAsync(
                user.Id, user.Email, user.UserName, user.FullName, roles, cancellationToken);

            var rawRefreshToken = tokenGenerator.GenerateRefreshToken();
            var refreshToken = new RefreshTokenEntity
            {
                UserId = user.Id,
                TokenHash = tokenGenerator.HashToken(rawRefreshToken),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
            };

            await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto(
                access.AccessToken,
                access.ExpiresAtUtc,
                rawRefreshToken,
                refreshToken.ExpiresAtUtc);
        }
    }
}
