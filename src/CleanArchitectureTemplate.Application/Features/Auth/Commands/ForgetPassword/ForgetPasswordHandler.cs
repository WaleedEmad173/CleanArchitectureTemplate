using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgetPassword
{
    public sealed class ForgetPasswordHandler(
        IIdentityService identityService,
        IEmailService emailService
        ) : IRequestHandler<ForgetPasswordCommand, bool>
    {
        public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.GetByEmailAsync(request.Request.Email, cancellationToken);

            if (user == null)
            {
                return true;
            }

            var token = await identityService.GeneratePasswordResetTokenAsync(user.Id, cancellationToken);

            await emailService.SendEmailAsync(user.Email, "Password Reset",
            $"Your password reset token is: {token}");

            return true;
        }
    }
}
