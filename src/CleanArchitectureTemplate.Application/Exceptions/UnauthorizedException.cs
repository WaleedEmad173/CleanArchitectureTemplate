namespace CleanArchitectureTemplate.Application.Exceptions;

public sealed class UnauthorizedException(string message = "Unauthorized.")
    : Exception(message);
