using CleanArchitectureTemplate.API.Controllers.Base;
using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Features.Products.Commands.Create;
using CleanArchitectureTemplate.Application.Features.Products.Commands.Delete;
using CleanArchitectureTemplate.Application.Features.Products.Commands.Update;
using CleanArchitectureTemplate.Application.Features.Products.DTOs;
using CleanArchitectureTemplate.Application.Features.Products.Queries.GetAll;
using CleanArchitectureTemplate.Application.Features.Products.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers;

[Route("api/products")]
public sealed class ProductController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParams query, CancellationToken cancellationToken) =>
        FromPagedResult(await Mediator.Send(new GetProductsQuery(query), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new GetProductByIdQuery(id), cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto request, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new CreateProductCommand(request), cancellationToken), "Product created successfully.", StatusCodes.Status201Created);

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto request, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new UpdateProductCommand(id, request), cancellationToken), "Product updated successfully.");

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new DeleteProductCommand(id), cancellationToken), "Product deleted successfully.");
}
