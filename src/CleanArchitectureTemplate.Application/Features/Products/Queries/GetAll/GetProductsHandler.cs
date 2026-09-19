using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed class GetProductsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await unitOfWork.Products.GetPagedAsync(
            request.Params.PageNumber,
            request.Params.PageSize,
            cancellationToken);

        var data = items
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Price,
                x.Stock))
            .ToList();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.Params.PageSize);

        var meta = new PaginationMeta(
            request.Params.PageNumber,
            request.Params.PageSize,
            totalCount,
            totalPages);

        return new PagedResult<ProductDto>(data, meta);
    }
}
