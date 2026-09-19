using CleanArchitectureTemplate.Application.Features.Products.Commands.Update;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Products.Validators;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Request.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
