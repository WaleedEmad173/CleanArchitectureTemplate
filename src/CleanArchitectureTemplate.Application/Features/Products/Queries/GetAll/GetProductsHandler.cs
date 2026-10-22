using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Common.Querying;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed class GetProductsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var p = request.Params;

        return unitOfWork.Products.Query()
            .AsNoTracking()
            .Search(p.Search, nameof(Product.Name))
            .Filter(p.MinPrice, x => x.Price >= p.MinPrice)
            .Filter(p.MaxPrice, x => x.Price <= p.MaxPrice)
            .WhereIf(p.InStockOnly == true, x => x.Stock > 0)
            .Sort(
                p.SortBy,
                p.SortDescending,
                nameof(Product.Id),
                nameof(Product.Name),
                nameof(Product.Price),
                nameof(Product.Stock))
            .ToPagedResultAsync(p.PageNumber, p.PageSize, x => new ProductDto(x.Id, x.Name, x.Price, x.Stock), cancellationToken);
    }
}
