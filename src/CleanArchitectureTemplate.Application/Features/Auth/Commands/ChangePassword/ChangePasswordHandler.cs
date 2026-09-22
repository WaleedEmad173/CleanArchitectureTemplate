using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedException();

        var result = await identityService.ChangePasswordAsync(
            userId,
            request.Request.CurrentPassword,
            request.Request.NewPassword,
            cancellationToken);

        if (!result.Succeeded)
            throw new BadRequestException(string.Join(' ', result.Errors));

        return true;
    }
}
