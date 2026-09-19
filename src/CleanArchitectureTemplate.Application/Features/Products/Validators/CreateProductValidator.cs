using CleanArchitectureTemplate.Application.Features.Products.Commands.Create;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Products.Validators;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Request.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
