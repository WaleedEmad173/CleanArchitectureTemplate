using CleanArchitectureTemplate.Application.Features.Auth.Commands.RefreshToken;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Request.RefreshToken)
            .NotEmpty();
    }
}
