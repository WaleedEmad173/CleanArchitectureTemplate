using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
}
