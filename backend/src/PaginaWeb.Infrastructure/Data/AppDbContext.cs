using Microsoft.EntityFrameworkCore;
using PaginaWeb.Core.Entities;

namespace PaginaWeb.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            b.Property(x => x.Role).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Client>(b =>
        {
            b.ToTable("clients");
            b.HasKey(x => x.Id);
            b.Property(x => x.DocumentType).HasMaxLength(20).IsRequired();
            b.Property(x => x.DocumentNumber).HasMaxLength(25).IsRequired();
            b.Property(x => x.FirstName).HasMaxLength(120).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(120).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.Phone).HasMaxLength(30);
            b.Property(x => x.AddressLine1).HasMaxLength(200);
            b.Property(x => x.City).HasMaxLength(120);
            b.Property(x => x.Notes).HasMaxLength(800);
            b.HasIndex(x => x.DocumentNumber).IsUnique();
            b.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(x => x.Id);
            b.Property(x => x.Sku).HasMaxLength(50).IsRequired();
            b.Property(x => x.Name).HasMaxLength(160).IsRequired();
            b.Property(x => x.Category).HasMaxLength(80);
            b.Property(x => x.Description).HasMaxLength(1200);
            b.Property(x => x.Barcode).HasMaxLength(80);
            b.Property(x => x.ImageUrl).HasMaxLength(600);
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.HasIndex(x => x.Sku).IsUnique();
            b.HasIndex(x => x.Name);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.UpdatedAtUtc = now;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
