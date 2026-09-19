using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Create;

public sealed record CreateProductCommand(CreateProductDto Request) : IRequest<ProductDto>;
