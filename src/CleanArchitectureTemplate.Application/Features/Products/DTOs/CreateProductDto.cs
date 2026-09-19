namespace CleanArchitectureTemplate.Application.Features.Products.DTOs;

public sealed record CreateProductDto(
    string Name,
    decimal Price,
    int Stock);
