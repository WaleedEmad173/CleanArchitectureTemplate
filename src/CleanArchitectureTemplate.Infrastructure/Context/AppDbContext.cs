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

        builder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.HasIndex(x => x.Name);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.TokenHash)
                .HasMaxLength(128)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
