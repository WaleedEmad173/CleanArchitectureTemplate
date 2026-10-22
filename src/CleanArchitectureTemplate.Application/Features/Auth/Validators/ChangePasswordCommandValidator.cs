using CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.")
            .MaximumLength(100).WithMessage("Current password cannot exceed 100 characters.");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("New password cannot exceed 100 characters.")
            .NotEqual(x => x.Request.CurrentPassword).WithMessage("New password must be different from current password.");
    }
}
