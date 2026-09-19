namespace CleanArchitectureTemplate.Application.Exceptions;

public sealed class BadRequestException(string message) : Exception(message);
