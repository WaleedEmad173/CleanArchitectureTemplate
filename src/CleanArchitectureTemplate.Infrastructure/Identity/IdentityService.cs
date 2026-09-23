using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
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
            throw new NotFoundException("User not found.");

        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task CreateAsync(
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
            throw new BadRequestException(
                string.Join(" | ", result.Errors.Select(x => x.Description)));

        if (!await roleManager.RoleExistsAsync("User"))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole<int>("User"));

            if (!roleResult.Succeeded)
                throw new BadRequestException(
                    string.Join(" | ", roleResult.Errors.Select(x => x.Description)));
        }

        var roleAssignment = await userManager.AddToRoleAsync(user, "User");

        if (!roleAssignment.Succeeded)
            throw new BadRequestException(
                string.Join(" | ", roleAssignment.Errors.Select(x => x.Description)));
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

    public async Task UpdateProfileAsync(
        int userId,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new NotFoundException("User not found.");

        user.FullName = fullName.Trim();

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(" | ", result.Errors.Select(x => x.Description)));
    }

    public async Task ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new NotFoundException("User not found.");

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            var isWrongPassword = result.Errors.Any(
                e => e.Code == "PasswordMismatch");

            if (isWrongPassword)
                throw new UnauthorizedException("Current password is incorrect.");

            throw new BadRequestException(
                string.Join(" | ", result.Errors.Select(x => x.Description)));
        }
    }

    public async Task ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        string confirmPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (newPassword != confirmPassword)
            throw new BadRequestException("Passwords do not match.");

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(" | ", result.Errors.Select(x => x.Description)));
    }

    private static IdentityUserModel Map(ApplicationUser user) =>
        new(
            user.Id,
            user.FullName,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty);
}