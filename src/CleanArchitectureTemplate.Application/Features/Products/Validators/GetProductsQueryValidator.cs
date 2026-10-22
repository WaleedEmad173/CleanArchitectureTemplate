using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Common.Querying;
using CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Products.Validators;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Params)
            .NotNull()
            .WithMessage("Query parameters must be provided.");

        When(x => x.Params is not null, () =>
        {
            RuleFor(x => x.Params.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.Params.PageSize)
                .InclusiveBetween(1, PaginationParams.MaxPageSize)
                .WithMessage($"Page size must be between 1 and {PaginationParams.MaxPageSize}.");

            RuleFor(x => x.Params.Search)
                .MaximumLength(QueryParams.MaxSearchLength)
                .WithMessage($"Search term cannot exceed {QueryParams.MaxSearchLength} characters.");

            RuleFor(x => x.Params.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Params.MinPrice.HasValue)
                .WithMessage("Minimum price cannot be negative.");

            RuleFor(x => x.Params.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Params.MaxPrice.HasValue)
                .WithMessage("Maximum price cannot be negative.");

            RuleFor(x => x.Params)
                .Must(p => !p.MinPrice.HasValue || !p.MaxPrice.HasValue || p.MinPrice.Value <= p.MaxPrice.Value)
                .WithMessage("Minimum price cannot be greater than maximum price.");
        });
    }
}
