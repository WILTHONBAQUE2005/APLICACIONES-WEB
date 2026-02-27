using HospitalManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Api.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
    {
    }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Doctor> Doctores => Set<Doctor>();
    public DbSet<CitaMedica> CitasMedicas => Set<CitaMedica>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
