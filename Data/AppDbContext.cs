
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Models;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("clientes");
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.DocumentNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(80).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.DocumentNumber).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Address>(e =>
        {
            e.ToTable("direcciones");
            e.HasKey(x => x.AddressId);
            e.Property(x => x.Line1).HasMaxLength(160).IsRequired();
            e.Property(x => x.City).HasMaxLength(80).IsRequired();
            e.Property(x => x.Province).HasMaxLength(80).IsRequired();
            e.Property(x => x.Country).HasMaxLength(80).IsRequired();
            e.Property(x => x.ZipCode).HasMaxLength(16);
            e.Property(x => x.IsDefault).HasDefaultValue(false);
            e.HasOne(x => x.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("categorias");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Description).HasMaxLength(240);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("productos");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.Sku).HasMaxLength(32).IsRequired();
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.Description).HasMaxLength(400);
            e.Property(x => x.UnitPrice).HasPrecision(10, 2);
            e.Property(x => x.Stock);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.Sku).IsUnique();
            e.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("pedidos");
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.OrderDate).HasColumnType("datetime");
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.PaymentStatus).HasConversion<int>();
            e.Property(x => x.Subtotal).HasPrecision(12, 2);
            e.Property(x => x.Discount).HasPrecision(12, 2);
            e.Property(x => x.Tax).HasPrecision(12, 2);
            e.Property(x => x.Total).HasPrecision(12, 2);
            e.Property(x => x.Notes).HasMaxLength(400);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.OrderNumber).IsUnique();

            e.HasOne(x => x.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("pedidodetalles");
            e.HasKey(x => x.OrderItemId);
            e.Property(x => x.Quantity);
            e.Property(x => x.UnitPrice).HasPrecision(12, 2);
            e.Property(x => x.LineTotal).HasPrecision(12, 2);
            e.HasOne(x => x.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.OrderId, x.ProductId });
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("pagos");
            e.HasKey(x => x.PaymentId);
            e.Property(x => x.Method).HasConversion<int>();
            e.Property(x => x.Amount).HasPrecision(12, 2);
            e.Property(x => x.PaidAt).HasColumnType("datetime");
            e.Property(x => x.Reference).HasMaxLength(60);
            e.HasOne(x => x.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Shipment>(e =>
        {
            e.ToTable("envios");
            e.HasKey(x => x.ShipmentId);
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.Carrier).HasMaxLength(80);
            e.Property(x => x.TrackingNumber).HasMaxLength(80);
            e.Property(x => x.ShipTo).HasMaxLength(200);
            e.Property(x => x.ShippedAt).HasColumnType("datetime");
            e.Property(x => x.DeliveredAt).HasColumnType("datetime");
            e.HasOne(x => x.Order)
                .WithOne(o => o.Shipment)
                .HasForeignKey<Shipment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.OrderId).IsUnique();
        });

        modelBuilder.Entity<InventoryMovement>(e =>
        {
            e.ToTable("inventariomovimientos");
            e.HasKey(x => x.InventoryMovementId);
            e.Property(x => x.Type).HasConversion<int>();
            e.Property(x => x.Quantity);
            e.Property(x => x.Reason).HasMaxLength(120);
            e.Property(x => x.Reference).HasMaxLength(40);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.HasOne(x => x.Product)
                .WithMany(p => p.InventoryMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.ProductId, x.CreatedAt });
        });
    }
}
