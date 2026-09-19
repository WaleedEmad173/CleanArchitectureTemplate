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

        var cacheKey = $"RegistrationOTP_{dto.Email.Trim().ToLower()}";

        if (!memoryCache.TryGetValue(cacheKey, out string? savedOtp))
            throw new BadRequestException("The verification code has expired or does not exist.");

        if (savedOtp != dto.OtpCode)
            throw new BadRequestException("Invalid verification code.");

        if (await identityService.EmailExistsAsync(dto.Email, cancellationToken))
            throw new ConflictException("Email is already registered.");

        var result = await identityService.CreateAsync(
            dto.FullName,
            dto.Email,
            dto.Email,
            dto.Password,
            cancellationToken);

        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors));

        var user = await identityService.GetByEmailAsync(dto.Email, cancellationToken)
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

        memoryCache.Remove(cacheKey);

        return new AuthResponseDto(
            access.AccessToken,
            access.ExpiresAtUtc,
            rawRefreshToken,
            refreshToken.ExpiresAtUtc);
    }
}