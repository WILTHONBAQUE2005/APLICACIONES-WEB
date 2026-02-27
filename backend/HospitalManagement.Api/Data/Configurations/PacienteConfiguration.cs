using HospitalManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Api.Data.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");

        builder.HasKey(x => x.PacienteId);

        builder.Property(x => x.PacienteId)
            .HasColumnName("paciente_id");

        builder.Property(x => x.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Apellido)
            .HasColumnName("apellido")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20)
            .IsRequired();
    }
}
