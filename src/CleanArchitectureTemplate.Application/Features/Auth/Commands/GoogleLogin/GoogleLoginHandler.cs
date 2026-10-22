using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;
using RefreshTokenEntity = CleanArchitectureTemplate.Domain.Entities.RefreshToken;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin;

public sealed class GoogleLoginHandler(
    IGoogleAuthService googleAuthService,
    IIdentityService identityService,
    IJwtService jwtService,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        GoogleLoginCommand request,
        CancellationToken cancellationToken)
    {
        GoogleUserInfo googleUser;
        try
        {
            googleUser = await googleAuthService.ValidateIdTokenAsync(
                request.Request.IdToken,
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch
        {
            throw new BadRequestException("Invalid Google token.");
        }

        if (!googleUser.EmailVerified ||
            string.IsNullOrWhiteSpace(googleUser.Email) ||
            string.IsNullOrWhiteSpace(googleUser.Subject))
        {
            throw new UnauthorizedException("Google account email must be verified.");
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await identityService.GetByGoogleSubjectAsync(googleUser.Subject, cancellationToken);
            if (user is null)
            {
                user = await identityService.GetByEmailAsync(googleUser.Email, cancellationToken);

                if (user is null)
                {
                    var randomPassword = Guid.NewGuid().ToString() + "Aa1@";
                    await identityService.CreateAsync(
                        googleUser.Name ?? googleUser.Email,
                        googleUser.Email,
                        googleUser.Email,
                        randomPassword,
                        cancellationToken);

                    user = await identityService.GetByEmailAsync(googleUser.Email, cancellationToken)
                        ?? throw new BadRequestException("Google account could not be created.");
                }

                await identityService.LinkGoogleLoginAsync(user.Id, googleUser.Subject, cancellationToken);
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

            await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return new AuthResponseDto(
                access.AccessToken,
                access.ExpiresAtUtc,
                rawRefreshToken,
                refreshToken.ExpiresAtUtc);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
