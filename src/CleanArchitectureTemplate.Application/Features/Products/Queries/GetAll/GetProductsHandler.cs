using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed class GetProductsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var p = request.Params;

        return await unitOfWork.Products
            .Query()
            .AsNoTracking()
            .Filter(p.MinPrice, x => x.Price >= p.MinPrice)
            .Filter(p.MaxPrice, x => x.Price <= p.MaxPrice)
            .Filter(p.InStockOnly == true, x => x.Stock > 0)
            .Search(p.Search, x => x.Name)
            .Sort(p.SortBy, p.IsDescending, defaultProperty: nameof(Product.Id))
            .ToPagedResultAsync(
                p.PageNumber,
                p.PageSize,
                x => new ProductDto(x.Id, x.Name, x.Price, x.Stock),
                cancellationToken);
    }
}
