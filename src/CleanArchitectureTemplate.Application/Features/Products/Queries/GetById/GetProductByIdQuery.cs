using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetById;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
