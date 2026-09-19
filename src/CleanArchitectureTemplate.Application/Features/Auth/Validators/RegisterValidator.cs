using CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Request.FullName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Request.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.Request.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(100);

        RuleFor(x => x.Request.ConfirmPassword)
            .Equal(x => x.Request.Password)
            .WithMessage("Passwords do not match.");
    }
}
