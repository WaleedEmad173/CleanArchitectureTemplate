using CleanArchitectureTemplate.Application.Common.Querying;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed class ProductQueryParams : QueryParams
{
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? InStockOnly { get; init; }
}
