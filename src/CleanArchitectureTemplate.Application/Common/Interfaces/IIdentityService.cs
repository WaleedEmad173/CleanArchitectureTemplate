namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public sealed record IdentityUserModel(
    int Id,
    string FullName,
    string UserName,
    string Email);

public sealed record IdentityOperationResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors);

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

    Task<IdentityOperationResult> CreateAsync(
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

    Task<IdentityOperationResult> UpdateProfileAsync(
        int userId,
        string fullName,
        CancellationToken cancellationToken = default);

    Task<IdentityOperationResult> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
