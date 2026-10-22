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
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .MaximumLength(150);

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(9999999999999999.99m)
            .Must(price => decimal.Round(price, 2) == price)
            .WithMessage("Price must be between 0 and 9999999999999999.99 with at most 2 decimal places.");

        RuleFor(x => x.Request.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
