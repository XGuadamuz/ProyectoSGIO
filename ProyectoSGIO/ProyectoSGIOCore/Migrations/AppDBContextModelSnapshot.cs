﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoSGIOCore.Data;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProyectoSGIOCore.Models.Archivo", b =>
                {
                    b.Property<int>("IdArchivo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdArchivo"));

                    b.Property<DateTime>("FechaSubida")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("IdArchivo");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Archivos");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.CierreFinanciero", b =>
                {
                    b.Property<int>("IdCierre")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCierre"));

                    b.Property<int>("Anio")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCierre")
                        .HasColumnType("datetime2");

                    b.Property<int>("Mes")
                        .HasColumnType("int");

                    b.Property<string>("Observaciones")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TipoCierre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalEgresos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalIngresos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Utilidad")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdCierre");

                    b.ToTable("CierresFinancieros");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Empleado", b =>
                {
                    b.Property<int>("IdEmpleado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEmpleado"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdEmpleado");

                    b.ToTable("Empleado", (string)null);
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FacturaProveedor", b =>
                {
                    b.Property<int>("IdFactura")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdFactura"));

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaEmision")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdProveedor")
                        .HasColumnType("int");

                    b.Property<decimal>("MontoTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("NumeroFactura")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProveedorIdProveedor")
                        .HasColumnType("int");

                    b.HasKey("IdFactura");

                    b.HasIndex("ProveedorIdProveedor");

                    b.ToTable("Facturas");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Fase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProyectoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProyectoId");

                    b.ToTable("Fases");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseControl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PuntoControlId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PuntoControlId");

                    b.ToTable("FasesControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseInicial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlanInicialId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlanInicialId");

                    b.ToTable("FasesIniciales");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Hito", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<int>("ProyectoId")
                        .HasColumnType("int");

                    b.Property<int>("estado")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("IdUsuario");

                    b.HasIndex("ProyectoId");

                    b.ToTable("Hitos");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.ImpactoMedidaCorrectiva", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Fase")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaImplementacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Impacto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medida")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProyectoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProyectoId");

                    b.ToTable("ImpactosMedidasCorrectivas");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Inventario", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<string>("Categoria")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InformacionAdicional")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PrecioTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PrecioUnidad")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("Inventarios");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PlanInicial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProyectoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProyectoId");

                    b.ToTable("PlanesIniciales");
                });

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

            modelBuilder.Entity("ProyectoSGIOCore.Models.Proyecto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Proyectos");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PuntoControl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaRegistro")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProyectoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProyectoId");

                    b.ToTable("PuntosControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Reporte", b =>
                {
                    b.Property<int>("IdReporte")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdReporte"));

                    b.Property<DateTime>("FechaSubida")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("IdReporte");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Reportes");
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

            modelBuilder.Entity("ProyectoSGIOCore.Models.Tarea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Completada")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Costo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FaseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FaseId");

                    b.ToTable("Tareas");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.TareaControl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Completada")
                        .HasColumnType("bit");

                    b.Property<int>("FaseControlId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FaseControlId");

                    b.ToTable("TareasControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.TareaInicial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FaseInicialId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FaseInicialId");

                    b.ToTable("TareasIniciales");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<bool>("Activo")
                        .HasMaxLength(1)
                        .HasColumnType("bit");

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

                    b.Property<bool>("Temporal")
                        .HasMaxLength(1)
                        .HasColumnType("bit");

                    b.Property<bool>("TwoFA")
                        .HasMaxLength(1)
                        .HasColumnType("bit");

                    b.HasKey("IdUsuario");

                    b.HasIndex("IdRol");

                    b.ToTable("Usuario", (string)null);

                    b.HasData(
                        new
                        {
                            IdUsuario = 1,
                            Activo = true,
                            Apellido = "User",
                            Clave = "PTOmGpZfmyerAqbyAbnVHw==",
                            Correo = "admin@example.com",
                            IdRol = 1,
                            Nombre = "Admin",
                            Temporal = false,
                            TwoFA = false
                        },
                        new
                        {
                            IdUsuario = 2,
                            Activo = true,
                            Apellido = "User",
                            Clave = "PTOmGpZfmyerAqbyAbnVHw==",
                            Correo = "supervisor@example.com",
                            IdRol = 2,
                            Nombre = "Supervisor",
                            Temporal = false,
                            TwoFA = false
                        },
                        new
                        {
                            IdUsuario = 3,
                            Activo = true,
                            Apellido = "User",
                            Clave = "PTOmGpZfmyerAqbyAbnVHw==",
                            Correo = "empleado@example.com",
                            IdRol = 3,
                            Nombre = "Empleado",
                            Temporal = false,
                            TwoFA = false
                        });
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Archivo", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Usuario", "Usuario")
                        .WithMany("Archivos")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FacturaProveedor", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Proveedor", "Proveedor")
                        .WithMany()
                        .HasForeignKey("ProveedorIdProveedor");

                    b.Navigation("Proveedor");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Fase", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Proyecto", "Proyecto")
                        .WithMany("Fases")
                        .HasForeignKey("ProyectoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proyecto");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseControl", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.PuntoControl", "PuntoControl")
                        .WithMany("FasesControl")
                        .HasForeignKey("PuntoControlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PuntoControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseInicial", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.PlanInicial", "PlanInicial")
                        .WithMany("FasesIniciales")
                        .HasForeignKey("PlanInicialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlanInicial");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Hito", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("IdUsuario");

                    b.HasOne("ProyectoSGIOCore.Models.Proyecto", "Proyecto")
                        .WithMany("Hitos")
                        .HasForeignKey("ProyectoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proyecto");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.ImpactoMedidaCorrectiva", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Proyecto", "Proyecto")
                        .WithMany()
                        .HasForeignKey("ProyectoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proyecto");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PlanInicial", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Proyecto", "Proyecto")
                        .WithMany()
                        .HasForeignKey("ProyectoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proyecto");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Proyecto", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("IdUsuario");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PuntoControl", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Proyecto", "Proyecto")
                        .WithMany()
                        .HasForeignKey("ProyectoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proyecto");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Reporte", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Tarea", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.Fase", "Fase")
                        .WithMany("Tareas")
                        .HasForeignKey("FaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fase");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.TareaControl", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.FaseControl", "FaseControl")
                        .WithMany("TareasControl")
                        .HasForeignKey("FaseControlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FaseControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.TareaInicial", b =>
                {
                    b.HasOne("ProyectoSGIOCore.Models.FaseInicial", "FaseInicial")
                        .WithMany("TareasIniciales")
                        .HasForeignKey("FaseInicialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FaseInicial");
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

            modelBuilder.Entity("ProyectoSGIOCore.Models.Fase", b =>
                {
                    b.Navigation("Tareas");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseControl", b =>
                {
                    b.Navigation("TareasControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.FaseInicial", b =>
                {
                    b.Navigation("TareasIniciales");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PlanInicial", b =>
                {
                    b.Navigation("FasesIniciales");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Proyecto", b =>
                {
                    b.Navigation("Fases");

                    b.Navigation("Hitos");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.PuntoControl", b =>
                {
                    b.Navigation("FasesControl");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Rol", b =>
                {
                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("ProyectoSGIOCore.Models.Usuario", b =>
                {
                    b.Navigation("Archivos");
                });
#pragma warning restore 612, 618
        }
    }
}
