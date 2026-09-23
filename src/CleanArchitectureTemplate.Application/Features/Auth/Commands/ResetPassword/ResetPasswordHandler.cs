using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPassword
{
    internal class ResetPasswordHandler(
        IIdentityService identityService

        ) : IRequestHandler<ResetPasswordCommand, bool>
    {
        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await identityService.ResetPasswordAsync(request.Request.Email, request.Request.Token, request.Request.NewPassword, request.Request.ConfirmPassword, cancellationToken);

            return true;
        }
    }
}
