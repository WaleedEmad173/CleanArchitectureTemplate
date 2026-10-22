using CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .NotEqual(x => x.Request.CurrentPassword).WithMessage("New password must be different from current password.");
    }
}
