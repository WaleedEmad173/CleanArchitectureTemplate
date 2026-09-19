using CleanArchitectureTemplate.Application.Features.Auth.Commands.UpdateProfile;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Validators;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Request.FullName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Request.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);
    }
}
