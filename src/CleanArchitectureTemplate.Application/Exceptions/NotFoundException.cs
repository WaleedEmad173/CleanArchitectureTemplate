namespace CleanArchitectureTemplate.Application.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string resource, int id) : base($"{resource} with id {id} not found!")
    {
    }
    public NotFoundException(string message) : base(message)
    {
    }
}


