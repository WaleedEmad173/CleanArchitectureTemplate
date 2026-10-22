using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using FluentValidation;
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
        var user = await userManager.FindByEmailAsync(email.Trim());
        return user is null ? null : Map(user);
    }

    public async Task<IdentityUserModel?> GetByGoogleSubjectAsync(
        string subject,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByLoginAsync("Google", subject);
        return user is null ? null : Map(user);
    }

    public async Task LinkGoogleLoginAsync(
        int userId,
        string subject,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new NotFoundException("User not found.");

        var result = await userManager.AddLoginAsync(
            user,
            new UserLoginInfo("Google", subject, "Google"));

        if (!result.Succeeded)
            throw new ConflictException(string.Join(" | ", result.Errors.Select(error => error.Description)));
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email.Trim());
        return userManager.Users.AnyAsync(
            user => user.NormalizedEmail == normalizedEmail,
            cancellationToken);
    }

    public Task<bool> UserNameExistsAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userManager.NormalizeName(userName.Trim());
        return userManager.Users.AnyAsync(
            user => user.NormalizedUserName == normalizedUserName,
            cancellationToken);
    }

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
            LockoutEnabled = true,
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error =>
                    error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                throw new ConflictException("An account with this email already exists.");
            }

            throw new BadRequestException(
                string.Join(" | ", result.Errors.Select(x => x.Description)));
        }

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
        if (user is null)
            return false;

        if (!user.LockoutEnabled)
        {
            var lockoutResult = await userManager.SetLockoutEnabledAsync(user, true);
            if (!lockoutResult.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", lockoutResult.Errors.Select(error => error.Description)));
        }

        if (await userManager.IsLockedOutAsync(user))
            return false;

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            return false;
        }

        if (user.AccessFailedCount > 0)
            await userManager.ResetAccessFailedCountAsync(user);
        return true;
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
        {
            var failures = result.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Code, e.Description));

            throw new ValidationException("Validation failed.", failures);
        }
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

            var failures = result.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Code, e.Description));

            throw new ValidationException("Validation failed.", failures);
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
            throw new BadRequestException("Invalid or expired password reset token.");

        if (newPassword != confirmPassword)
            throw new BadRequestException("Passwords do not match.");

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code == "InvalidToken"))
                throw new BadRequestException("Invalid or expired password reset token.");

            var failures = result.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Code, e.Description));

            throw new ValidationException("Validation failed.", failures);
        }
    }

    private static IdentityUserModel Map(ApplicationUser user) =>
        new(
            user.Id,
            user.FullName,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty);
}
