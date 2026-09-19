using CleanArchitectureTemplate.Application.Common.Responses;
using CleanArchitectureTemplate.Application.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace CleanArchitectureTemplate.API.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
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

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Succeeded = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
