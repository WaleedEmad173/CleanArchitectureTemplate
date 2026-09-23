namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs
{
    public sealed record ResetPasswordRequestDto(
        string Email,
        string Token,
        string NewPassword,
        string ConfirmPassword);
}
