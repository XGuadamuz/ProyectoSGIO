using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class AddImpactoMedidaCorrectiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpactosMedidasCorrectivas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Fase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Medida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaImplementacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Impacto = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactosMedidasCorrectivas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpactosMedidasCorrectivas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpactosMedidasCorrectivas_ProyectoId",
                table: "ImpactosMedidasCorrectivas",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpactosMedidasCorrectivas");
        }
    }
}
