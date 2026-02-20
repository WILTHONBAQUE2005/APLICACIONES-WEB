using Microsoft.EntityFrameworkCore;
using NetCoreApi.Models;

namespace NetCoreApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<ClienteModel> Clientes { get; set; }
    public DbSet<AlumnoModel> Alumnos { get; set; }
    public DbSet<UsuarioModel> Usuarios { get; set; }
}
