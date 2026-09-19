using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Update;

public sealed record UpdateProductCommand(int Id, UpdateProductDto Request) : IRequest<ProductDto>;
