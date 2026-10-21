using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed record GetProductsQuery(ProductParams Params)
    : IRequest<PagedResult<ProductDto>>, ICacheableQuery
{
    public string CacheKey => $"ProductsList_{Params.PageNumber}_{Params.PageSize}_{Params.Search}_{Params.SortBy}_{Params.IsDescending}_{Params.MinPrice}_{Params.MaxPrice}_{Params.InStockOnly}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(5);
}
