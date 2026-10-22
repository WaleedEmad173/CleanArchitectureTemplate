using CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(x => x.Request.IdToken)
            .NotEmpty().WithMessage("Google ID token is required.");
    }
}
