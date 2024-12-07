using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanesIniciales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesIniciales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanesIniciales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FasesIniciales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanInicialId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FasesIniciales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FasesIniciales_PlanesIniciales_PlanInicialId",
                        column: x => x.PlanInicialId,
                        principalTable: "PlanesIniciales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TareasIniciales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaseInicialId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareasIniciales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareasIniciales_FasesIniciales_FaseInicialId",
                        column: x => x.FaseInicialId,
                        principalTable: "FasesIniciales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FasesIniciales_PlanInicialId",
                table: "FasesIniciales",
                column: "PlanInicialId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesIniciales_ProyectoId",
                table: "PlanesIniciales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_TareasIniciales_FaseInicialId",
                table: "TareasIniciales",
                column: "FaseInicialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareasIniciales");

            migrationBuilder.DropTable(
                name: "FasesIniciales");

            migrationBuilder.DropTable(
                name: "PlanesIniciales");
        }
    }
}
