using CleanArchitectureTemplate.Application.Common.Interfaces;
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
            var email = request.Request.Email.Trim();
            var emailExists = await identityService.EmailExistsAsync(email, cancellationToken);
            if (emailExists)
                return true;

            var otpCode = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var cacheKey = $"RegistrationOTP_{email.ToLowerInvariant()}";
            memoryCache.Remove($"{cacheKey}_Attempts");

            memoryCache.Set(
                cacheKey,
                otpCode,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSize(1));

            await emailService.SendEmailAsync(
                to: email,
                subject: "Registration Verification Code",
                body: $"Your verification code is: <b>{otpCode}</b>. It is valid for 5 minutes.",
                cancellationToken: cancellationToken);

            return true;
        }
    }
}
