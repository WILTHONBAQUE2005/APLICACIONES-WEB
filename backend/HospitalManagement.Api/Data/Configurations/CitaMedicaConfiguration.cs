using HospitalManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Api.Data.Configurations;

public class CitaMedicaConfiguration : IEntityTypeConfiguration<CitaMedica>
{
    public void Configure(EntityTypeBuilder<CitaMedica> builder)
    {
        builder.ToTable("CitasMedicas");

        builder.HasKey(x => x.CitaId);

        builder.Property(x => x.CitaId)
            .HasColumnName("cita_id");

        builder.Property(x => x.PacienteId)
            .HasColumnName("paciente_id")
            .IsRequired();

        builder.Property(x => x.DoctorId)
            .HasColumnName("doctor_id")
            .IsRequired();

        builder.Property(x => x.FechaHora)
            .HasColumnName("fecha_hora")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.Motivo)
            .HasColumnName("motivo")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Estado)
            .HasColumnName("estado")
            .HasMaxLength(20)
            .HasDefaultValue("Programada")
            .IsRequired();

        builder.HasIndex(x => x.PacienteId)
            .HasDatabaseName("IX_CitasMedicas_Paciente");

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_CitasMedicas_Doctor");

        builder.HasIndex(x => new { x.DoctorId, x.FechaHora })
            .HasDatabaseName("IX_CitasMedicas_Doctor_FechaHora");

        builder.HasIndex(x => new { x.PacienteId, x.FechaHora })
            .HasDatabaseName("IX_CitasMedicas_Paciente_FechaHora");

        builder.HasOne(x => x.Paciente)
            .WithMany(x => x.CitasMedicas)
            .HasForeignKey(x => x.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.CitasMedicas)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
