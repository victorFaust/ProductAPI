using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Products.Domain.Entities;

namespace Products.Infrastructure.Persistence.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Description).HasMaxLength(1000);
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
            e.Property(p => p.Colour).IsRequired().HasMaxLength(100);
            e.Property(p => p.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Token).IsRequired().HasMaxLength(512);
            e.Property(r => r.Username).IsRequired().HasMaxLength(200);
            e.Property(r => r.ExpiresAt).IsRequired();
            e.Property(r => r.CreatedAt).IsRequired();
            // Store roles as a comma-separated string with a value comparer
            var roleComparer = new ValueComparer<string[]>(
                (a, b) => a!.SequenceEqual(b!),
                c => c.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                c => c.ToArray());

            e.Property(r => r.Roles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Length == 0 ? Array.Empty<string>() : v.Split(',', StringSplitOptions.None),
                    roleComparer);
            e.HasIndex(r => r.Token).IsUnique();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(200);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
            e.Property(u => u.CreatedAt).IsRequired();

            var roleComparer = new ValueComparer<string[]>(
                (a, b) => a!.SequenceEqual(b!),
                c => c.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                c => c.ToArray());

            e.Property(u => u.Roles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Length == 0 ? Array.Empty<string>() : v.Split(',', StringSplitOptions.None),
                    roleComparer);

            e.HasIndex(u => u.Username).IsUnique();
        });
    }
}
