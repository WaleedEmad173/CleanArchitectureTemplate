namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public sealed record IdentityUserModel(
    int Id,
    string FullName,
    string UserName,
    string Email);

public interface IIdentityService
{
    Task<IdentityUserModel?> GetByIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<IdentityUserModel?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> UserNameExistsAsync(
        string userName,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        string fullName,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> CheckPasswordAsync(
        int userId,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetRolesAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task UpdateProfileAsync(
        int userId,
        string fullName,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<string> GeneratePasswordResetTokenAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        string Email,
        string token,
        string newPassword,
        string ConfirmPassword,
        CancellationToken cancellationToken = default);
}