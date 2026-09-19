using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Update;

public sealed class UpdateProductHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products
            .GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        product.Name = request.Request.Name.Trim();
        product.Price = request.Request.Price;
        product.Stock = request.Request.Stock;
        product.UpdatedAtUtc = DateTime.UtcNow;

        unitOfWork.Products.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock);
    }
}
