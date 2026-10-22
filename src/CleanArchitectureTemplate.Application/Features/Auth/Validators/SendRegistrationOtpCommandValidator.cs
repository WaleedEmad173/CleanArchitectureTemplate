using CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class SendRegistrationOtpCommandValidator : AbstractValidator<SendRegistrationOtpCommand>
{
    public SendRegistrationOtpCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(256);
    }
}
