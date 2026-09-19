using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;

public sealed record GetProductsQuery(PaginationParams Params)
    : IRequest<PagedResult<ProductDto>>;
