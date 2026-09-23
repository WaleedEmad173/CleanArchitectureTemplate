using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CleanArchitectureTemplate.Infrastructure.Context;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.ApplySoftDeleteQueryFilter();
    }
}
