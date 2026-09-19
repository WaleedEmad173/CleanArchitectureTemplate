using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.UpdateProfile;

public sealed class UpdateProfileHandler(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedException();

        var result = await identityService.UpdateProfileAsync(
            userId,
            request.Request.FullName,
            cancellationToken);

        if (!result.Succeeded)
            throw new ConflictException(string.Join(" ", result.Errors));

        var user = await identityService.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var roles = await identityService.GetRolesAsync(user.Id, cancellationToken);

        return new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            roles);
    }
}
