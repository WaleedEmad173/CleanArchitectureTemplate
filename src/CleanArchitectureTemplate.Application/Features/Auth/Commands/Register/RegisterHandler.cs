using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using RefreshTokenEntity = CleanArchitectureTemplate.Domain.Entities.RefreshToken;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;

public sealed class RegisterHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork,
    IMemoryCache memoryCache)
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;

        var normalizedEmail = dto.Email.Trim();
        var cacheKey = $"RegistrationOTP_{normalizedEmail.ToLowerInvariant()}";
        var attemptsKey = $"{cacheKey}_Attempts";

        if (!memoryCache.TryGetValue(cacheKey, out string? savedOtp))
            throw new BadRequestException("The verification code has expired or does not exist.");

        if (savedOtp != dto.OtpCode)
        {
            var attempts = memoryCache.TryGetValue(attemptsKey, out int existingAttempts)
                ? existingAttempts + 1
                : 1;
            memoryCache.Set(
                attemptsKey,
                attempts,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSize(1));

            if (attempts >= 5)
            {
                memoryCache.Remove(cacheKey);
                memoryCache.Remove(attemptsKey);
                throw new BadRequestException("Too many invalid verification code attempts. Request a new code.");
            }

            throw new BadRequestException("Invalid verification code.");
        }

        if (await identityService.EmailExistsAsync(normalizedEmail, cancellationToken))
            throw new ConflictException("Email is already registered.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await identityService.CreateAsync(
                dto.FullName,
                normalizedEmail,
                normalizedEmail,
                dto.Password,
                cancellationToken);

            var user = await identityService.GetByEmailAsync(normalizedEmail, cancellationToken)
                ?? throw new BadRequestException("User could not be created.");

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

            memoryCache.Remove(cacheKey);
            memoryCache.Remove(attemptsKey);

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
