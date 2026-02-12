using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaginaWeb.Infrastructure.Data;

#nullable disable

namespace PaginaWeb.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder
                .HasAnnotation("MySql:CharSet", "utf8mb4");

            modelBuilder.Entity("PaginaWeb.Core.Entities.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)")
                        .HasCollation("ascii_general_ci");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasCharSet("utf8mb4");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastLoginAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasCharSet("utf8mb4");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("users");
                });

            modelBuilder.Entity("PaginaWeb.Core.Entities.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)")
                        .HasCollation("ascii_general_ci");

                    b.Property<string>("AddressLine1")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("City")
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasCharSet("utf8mb4");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DocumentNumber")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("DocumentType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasCharSet("utf8mb4");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("Notes")
                        .HasMaxLength(800)
                        .HasColumnType("varchar(800)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasCharSet("utf8mb4");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("DocumentNumber")
                        .IsUnique();

                    b.HasIndex("Email");

                    b.ToTable("clients");
                });

            modelBuilder.Entity("PaginaWeb.Core.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)")
                        .HasCollation("ascii_general_ci");

                    b.Property<string>("Barcode")
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("Category")
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasCharSet("utf8mb4");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(1200)
                        .HasColumnType("varchar(1200)")
                        .HasCharSet("utf8mb4");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(600)
                        .HasColumnType("varchar(600)")
                        .HasCharSet("utf8mb4");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("varchar(160)")
                        .HasCharSet("utf8mb4");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasCharSet("utf8mb4");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("Sku")
                        .IsUnique();

                    b.ToTable("products");
                });
        }
    }
}
