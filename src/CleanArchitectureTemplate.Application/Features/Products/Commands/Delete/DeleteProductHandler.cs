using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductHandler(IUnitOfWork unitOfWork, ICacheVersionStore cacheVersionStore)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products
            .GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product", request.Id);

        unitOfWork.Products.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        cacheVersionStore.Invalidate("ProductsList");
        return true;
    }
}
