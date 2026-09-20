using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP
{
    public class SendRegistrationOtpCommandHandler(
        IMemoryCache memoryCache,
        IEmailService emailService,
        IIdentityService identityService)
        : IRequestHandler<SendRegistrationOtpCommand, bool>
    {
        public async Task<bool> Handle(SendRegistrationOtpCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await identityService.EmailExistsAsync(request.Request.Email, cancellationToken);
            if (emailExists)
            {
                throw new BadRequestException("This email is already registered.");
            }

            var otpCode = new Random().Next(100000, 999999).ToString();
            var cacheKey = $"RegistrationOTP_{request.Request.Email.Trim().ToLower()}";

            memoryCache.Set(cacheKey, otpCode, TimeSpan.FromMinutes(5));

            await emailService.SendEmailAsync(
                to: request.Request.Email,
                subject: "Registration Verification Code",
                body: $"Your verification code is: <b>{otpCode}</b>. It is valid for 5 minutes.");

            return true;
        }
    }
}
