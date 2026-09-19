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
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination) =>
        FromResult((await Mediator.Send(new GetProductsQuery(pagination))).Items);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        FromResult(await Mediator.Send(new GetProductByIdQuery(id)));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto request) =>
        FromResult(await Mediator.Send(new CreateProductCommand(request)), "Product created successfully.", StatusCodes.Status201Created);

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto request) =>
        FromResult(await Mediator.Send(new UpdateProductCommand(id, request)), "Product updated successfully.");

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await Mediator.Send(new DeleteProductCommand(id)), "Product deleted successfully.");
}

