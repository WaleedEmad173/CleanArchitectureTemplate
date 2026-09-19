using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Queries.GetProfile;

public sealed class GetProfileHandler(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedException();

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
