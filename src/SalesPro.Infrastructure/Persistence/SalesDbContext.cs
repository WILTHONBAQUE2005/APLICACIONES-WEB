using Microsoft.EntityFrameworkCore;
using SalesPro.Domain.Entities;

namespace SalesPro.Infrastructure.Persistence;

public sealed class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Productos");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.ProductId).HasColumnName("producto_id");
            e.Property(x => x.Code).HasColumnName("codigo").HasMaxLength(32).IsRequired();
            e.Property(x => x.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            e.Property(x => x.Description).HasColumnName("descripcion").HasMaxLength(500);
            e.Property(x => x.Price).HasColumnName("precio").HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Stock).HasColumnName("stock").IsRequired();
            e.Property(x => x.IsActive).HasColumnName("activo").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("creado_en").IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("Clientes");
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.CustomerId).HasColumnName("cliente_id");
            e.Property(x => x.Cedula).HasColumnName("cedula").HasMaxLength(10).IsRequired();
            e.Property(x => x.FirstName).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            e.Property(x => x.LastName).HasColumnName("apellido").HasMaxLength(80).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasColumnName("telefono").HasMaxLength(30);
            e.Property(x => x.Address).HasColumnName("direccion").HasMaxLength(200);
            e.Property(x => x.IsActive).HasColumnName("activo").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("creado_en").IsRequired();

            e.HasIndex(x => x.Cedula).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Sale>(e =>
        {
            e.ToTable("Ventas");
            e.HasKey(x => x.SaleId);
            e.Property(x => x.SaleId).HasColumnName("venta_id");
            e.Property(x => x.InvoiceNumber).HasColumnName("numero_factura").HasMaxLength(30).IsRequired();
            e.Property(x => x.CustomerId).HasColumnName("cliente_id").IsRequired();
            e.Property(x => x.SaleDate).HasColumnName("fecha").IsRequired();

            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2).IsRequired();
            e.Property(x => x.VatRate).HasColumnName("iva_tasa").HasPrecision(5, 4).IsRequired();
            e.Property(x => x.VatAmount).HasColumnName("iva_valor").HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Total).HasColumnName("total").HasPrecision(18, 2).IsRequired();

            e.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.InvoiceNumber).IsUnique();
        });

        modelBuilder.Entity<SaleItem>(e =>
        {
            e.ToTable("VentaDetalles");
            e.HasKey(x => x.SaleItemId);
            e.Property(x => x.SaleItemId).HasColumnName("venta_detalle_id");
            e.Property(x => x.SaleId).HasColumnName("venta_id").IsRequired();
            e.Property(x => x.ProductId).HasColumnName("producto_id").IsRequired();
            e.Property(x => x.Quantity).HasColumnName("cantidad").IsRequired();
            e.Property(x => x.UnitPrice).HasColumnName("precio_unitario").HasPrecision(18, 2).IsRequired();
            e.Property(x => x.LineSubtotal).HasColumnName("subtotal").HasPrecision(18, 2).IsRequired();

            e.HasOne(x => x.Sale)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.SaleId, x.ProductId }).IsUnique();
        });
    }
}
