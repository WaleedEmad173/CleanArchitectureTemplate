using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductHandler(IUnitOfWork unitOfWork)
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
        return true;
    }
}
