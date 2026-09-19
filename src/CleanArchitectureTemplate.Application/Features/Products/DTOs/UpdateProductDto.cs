namespace CleanArchitectureTemplate.Application.Features.Products.DTOs;

public sealed record UpdateProductDto(
    string Name,
    decimal Price,
    int Stock);
