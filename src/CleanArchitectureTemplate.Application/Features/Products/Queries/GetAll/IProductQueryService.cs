using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public interface IProductQueryService
{
    Task<PagedResult<ProductDto>> GetAsync(
        ProductQueryParams parameters,
        CancellationToken cancellationToken = default);
}
