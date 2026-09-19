namespace CleanArchitectureTemplate.Application.Common.Responses;

public static class ResponseHandler
{
    public static ApiResponse<T> Success<T>(
        T data,
        string message = "Success",
        object? meta = null) =>
        new()
        {
            Succeeded = true,
            StatusCode = StatusCodesFor.Ok,
            Message = message,
            Data = data,
            Meta = meta
        };

    public static ApiResponse<T> Created<T>(
        T data,
        string message = "Created") =>
        new()
        {
            Succeeded = true,
            StatusCode = StatusCodesFor.Created,
            Message = message,
            Data = data
        };

    public static ApiResponse<object> NoContent(string message = "Success") =>
        new()
        {
            Succeeded = true,
            StatusCode = StatusCodesFor.NoContent,
            Message = message
        };

    private static class StatusCodesFor
    {
        public const int Ok = 200;
        public const int Created = 201;
        public const int NoContent = 204;
    }
}
