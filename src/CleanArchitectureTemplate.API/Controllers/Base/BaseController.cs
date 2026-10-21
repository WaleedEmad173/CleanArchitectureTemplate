using CleanArchitectureTemplate.Application.Common.Pagination;
using CleanArchitectureTemplate.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.Base;

[ApiController]
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator => mediator;

    protected IActionResult FromResult<T>(
        T data,
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        var response = new ApiResponse<T>
        {
            Succeeded = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

        return StatusCode(statusCode, response);
    }

    protected IActionResult FromPagedResult<T>(
        PagedResult<T> pagedResult,
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        var response = ResponseHandler.Success(
            data: pagedResult.Items,
            message: message,
            meta: pagedResult.Meta);

        return StatusCode(statusCode, response);
    }
}
