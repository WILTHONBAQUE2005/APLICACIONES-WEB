using HospitalManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Api.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctores");

        builder.HasKey(x => x.DoctorId);

        builder.Property(x => x.DoctorId)
            .HasColumnName("doctor_id");

        builder.Property(x => x.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Apellido)
            .HasColumnName("apellido")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Especialidad)
            .HasColumnName("especialidad")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20)
            .IsRequired();
    }
}
