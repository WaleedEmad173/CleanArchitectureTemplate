using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Queries.GetById;

public sealed class GetProductByIdHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products
            .GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock);
    }
}
