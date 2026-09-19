using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Create;

public sealed class CreateProductHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Request.Name.Trim(),
            Price = request.Request.Price,
            Stock = request.Request.Stock
        };

        await unitOfWork.Products
            .AddAsync(product, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock);
    }
}
