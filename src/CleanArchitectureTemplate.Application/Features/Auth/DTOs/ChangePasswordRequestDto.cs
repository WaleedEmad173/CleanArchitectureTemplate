namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs
{
    public sealed record ChangePasswordRequestDto(
        string CurrentPassword,
        string NewPassword
    );
}
