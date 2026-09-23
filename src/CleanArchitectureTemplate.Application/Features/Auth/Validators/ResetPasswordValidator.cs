using CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPassword;
using FluentValidation;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Request.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Request.ConfirmPassword)
            .Equal(x => x.Request.NewPassword).WithMessage("Passwords do not match.");
    }
}