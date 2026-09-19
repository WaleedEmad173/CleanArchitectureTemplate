namespace CleanArchitectureTemplate.Application.Common.Responses;

public sealed record ResponseError(
    string Code,
    string Message,
    string? Field = null);
