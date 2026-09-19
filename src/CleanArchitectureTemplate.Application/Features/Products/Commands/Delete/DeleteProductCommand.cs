using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Products.Commands.Delete;

public sealed record DeleteProductCommand(int Id) : IRequest<bool>;
