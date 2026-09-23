using CleanArchitectureTemplate.Application.Common.Responses;
using CleanArchitectureTemplate.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Validation failed.",
                    validationException.Errors
                        .Select(x => new ResponseError(
                            "Validation",
                            x.ErrorMessage,
                            x.PropertyName))
                        .ToArray()
                ),

            NotFoundException ex =>
                (StatusCodes.Status404NotFound, ex.Message, null),

            ConflictException ex =>
                (StatusCodes.Status409Conflict, ex.Message, null),

            BadRequestException ex =>
                (StatusCodes.Status400BadRequest, ex.Message, null),

            UnauthorizedException ex =>
                (StatusCodes.Status401Unauthorized, ex.Message, null),

            ForbiddenException ex =>
                (StatusCodes.Status403Forbidden, ex.Message, null),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    null
                )
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Succeeded = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
