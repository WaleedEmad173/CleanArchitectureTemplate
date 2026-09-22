using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<int>> roleManager)
    : IIdentityService
{
    public async Task<IdentityUserModel?> GetByIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user is null ? null : Map(user);
    }

    public async Task<IdentityUserModel?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is null ? null : Map(user);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        userManager.Users.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<bool> UserNameExistsAsync(
        string userName,
        CancellationToken cancellationToken = default) =>
        userManager.Users.AnyAsync(x => x.UserName == userName, cancellationToken);

    public async Task<string> GeneratePasswordResetTokenAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new InvalidOperationException("User not found.");

        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityOperationResult> CreateAsync(
        string fullName,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            FullName = fullName.Trim(),
            UserName = userName.Trim(),
            Email = email.Trim(),
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return new IdentityOperationResult(
                false,
                result.Errors.Select(x => x.Description).ToArray());

        if (!await roleManager.RoleExistsAsync("User"))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole<int>("User"));

            if (!roleResult.Succeeded)
                return new IdentityOperationResult(
                    false,
                    roleResult.Errors.Select(x => x.Description).ToArray());
        }

        var roleAssignment = await userManager.AddToRoleAsync(user, "User");

        if (!roleAssignment.Succeeded)
            return new IdentityOperationResult(
                false,
                roleAssignment.Errors.Select(x => x.Description).ToArray());

        return new IdentityOperationResult(true, Array.Empty<string>());
    }

    public async Task<bool> CheckPasswordAsync(
        int userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is not null &&
               await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IReadOnlyCollection<string>> GetRolesAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Array.Empty<string>();

        return (await userManager.GetRolesAsync(user)).ToArray();
    }

    public async Task<IdentityOperationResult> UpdateProfileAsync(
        int userId,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return new IdentityOperationResult(false, new[] { "User not found." });

        user.FullName = fullName.Trim();

        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? new IdentityOperationResult(true, Array.Empty<string>())
            : new IdentityOperationResult(
                false,
                result.Errors.Select(x => x.Description).ToArray());
    }

    public async Task<IdentityOperationResult> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return new IdentityOperationResult(false, new[] { "User not found." });

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        return result.Succeeded
            ? new IdentityOperationResult(true, Array.Empty<string>())
            : new IdentityOperationResult(
                false,
                result.Errors.Select(x => x.Description).ToArray());
    }

    private static IdentityUserModel Map(ApplicationUser user) =>
        new(
            user.Id,
            user.FullName,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty);
}
