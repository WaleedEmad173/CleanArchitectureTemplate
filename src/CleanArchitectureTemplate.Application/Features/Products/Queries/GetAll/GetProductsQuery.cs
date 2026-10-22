using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed record GetProductsQuery(ProductQueryParams Params)
    : IRequest<PagedResult<ProductDto>>, ICacheableQuery
{
    public string CacheKey =>
        $"ProductsList_p{Params.PageNumber}_s{Params.PageSize}_q{Params.Search?.Trim().ToLowerInvariant()}_sort{Params.SortBy?.Trim().ToLowerInvariant()}_desc{Params.SortDescending}_min{Params.MinPrice}_max{Params.MaxPrice}_stock{Params.InStockOnly}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(5);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(15);
}
