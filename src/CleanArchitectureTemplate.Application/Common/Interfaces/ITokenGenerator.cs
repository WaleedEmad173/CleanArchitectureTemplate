namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ITokenGenerator
{
    string GenerateRefreshToken();
    string HashToken(string token);
}
