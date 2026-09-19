namespace CleanArchitectureTemplate.Application.Common.Responses;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public object? Errors { get; init; }
    public object? Meta { get; init; }
}
