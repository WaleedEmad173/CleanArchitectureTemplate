using CleanArchitectureTemplate.Application.Common.Querying;
using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Infrastructure.Context;
using CleanArchitectureTemplate.Infrastructure.Querying;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Queries;

public sealed class ProductQueryService(AppDbContext context) : IProductQueryService
{
    public Task<PagedResult<ProductDto>> GetAsync(
        ProductQueryParams parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return context.Products
            .AsNoTracking()
            .Search(parameters.Search, nameof(Product.Name))
            .Filter(parameters.MinPrice, product => product.Price >= parameters.MinPrice)
            .Filter(parameters.MaxPrice, product => product.Price <= parameters.MaxPrice)
            .Filter(parameters.InStockOnly, product => product.Stock > 0)
            .Sort(
                parameters.SortBy,
                parameters.SortDescending,
                nameof(Product.Id),
                nameof(Product.Name),
                nameof(Product.Price),
                nameof(Product.Stock))
            .ToPagedResultAsync(
                parameters.PageNumber,
                parameters.PageSize,
                product => new ProductDto(product.Id, product.Name, product.Price, product.Stock),
                cancellationToken);
    }
}
