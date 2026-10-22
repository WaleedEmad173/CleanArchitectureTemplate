using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole<int>> roleManager)
    {
        foreach (var roleName in new[] { "User", "Admin" })
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (!result.Succeeded && !await roleManager.RoleExistsAsync(roleName))
                throw new InvalidOperationException(
                    $"Could not seed the {roleName} role: {string.Join(" | ", result.Errors.Select(error => error.Description))}");
        }
    }
}
