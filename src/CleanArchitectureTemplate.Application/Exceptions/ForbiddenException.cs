namespace CleanArchitectureTemplate.Application.Exceptions;

public sealed class ForbiddenException(string message = "Forbidden.")
    : Exception(message);
