using ClientesCrudWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientesCrudWeb.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<TipoCliente> TiposCliente => Set<TipoCliente>();
    public DbSet<Canton> Cantones => Set<Canton>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Cedula)
            .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Email)
            .IsUnique();

        
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Ciudad)
            .WithMany(x => x.Clientes)
            .HasForeignKey(c => c.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Canton>()
            .HasOne(x => x.Ciudad)
            .WithMany(c => c.Cantones)
            .HasForeignKey(x => x.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Canton)
            .WithMany(x => x.Clientes)
            .HasForeignKey(c => c.CantonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.TipoCliente)
            .WithMany(x => x.Clientes)
            .HasForeignKey(c => c.TipoClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Ciudad>().HasData(
            
            new Ciudad { Id = 1, Nombre = "Santo Domingo de los Tsáchilas", Provincia = "Santo Domingo de los Tsáchilas" }
        );

        modelBuilder.Entity<Canton>().HasData(
            
            new Canton { Id = 1, Nombre = "Santo Domingo", CiudadId = 1 },
            new Canton { Id = 2, Nombre = "La Concordia", CiudadId = 1 }
        );

        modelBuilder.Entity<TipoCliente>().HasData(
            new TipoCliente { Id = 1, Nombre = "Normal" },
            new TipoCliente { Id = 2, Nombre = "Preferente" },
            new TipoCliente { Id = 3, Nombre = "VIP" }
        );
    }
}
