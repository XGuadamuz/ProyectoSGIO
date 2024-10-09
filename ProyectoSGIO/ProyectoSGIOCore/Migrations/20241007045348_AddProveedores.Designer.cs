﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoSGIOCore.Data;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20241007045348_AddProveedores")]
    partial class AddProveedores
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProyectoSGIOCore.Models.Proveedor", b =>
                {
                    b.Property<int>("IdProveedor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProveedor"));

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("IdProveedor");

                    b.ToTable("Proveedor", (string)null);

                    b.HasData(
                        new
                        {
                            IdProveedor = 1,
                            Correo = "abc@proveedor.com",
                            Direccion = "Calle Falsa 123",
                            Estado = true,
                            Nombre = "Proveedor ABC",
                            Telefono = "1234567890"
                        },
                        new
                        {
                            IdProveedor = 2,
                            Correo = "xyz@proveedor.com",
                            Direccion = "Avenida Siempre Viva 742",
                            Estado = false,
                            Nombre = "Proveedor XYZ",
                            Telefono = "0987654321"
                        });
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Rol", b =>
                {
                    b.Property<int>("IdRol")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRol"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdRol");

                    b.ToTable("Rol", (string)null);

                    b.HasData(
                        new
                        {
                            IdRol = 1,
                            Nombre = "Administrador"
                        },
                        new
                        {
                            IdRol = 2,
                            Nombre = "Supervisor"
                        },
                        new
                        {
                            IdRol = 3,
                            Nombre = "Empleado"
                        },
                        new
                        {
                            IdRol = 4,
                            Nombre = "Usuario"
                        });
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("IdRol")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdUsuario");

                    b.HasIndex("IdRol");

                    b.ToTable("Usuario", (string)null);

                    b.HasData(
                        new
                        {
                            IdUsuario = 1,
                            Apellido = "User",
                            Clave = "123",
                            Correo = "admin@example.com",
                            IdRol = 1,
                            Nombre = "Admin"
                        },
                        new
                        {
                            IdUsuario = 2,
                            Apellido = "User",
                            Clave = "123",
                            Correo = "supervisor@example.com",
                            IdRol = 2,
                            Nombre = "Supervisor"
                        },
                        new
                        {
                            IdUsuario = 3,
                            Apellido = "User",
                            Clave = "123",
                            Correo = "empleado@example.com",
                            IdRol = 3,
                            Nombre = "Empleado"
                        });
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Usuario", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Rol", "Rol")
                        .WithMany("Usuarios")
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Rol", b =>
                {
                    b.Navigation("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
