using CleanArchitectureTemplate.Application.Features.Auth.Commands.Login;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Request.Password)
            .NotEmpty();
    }
}
