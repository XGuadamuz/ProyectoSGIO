using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class FinalProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Problemas_Proyectos_ProyectoId",
                table: "Problemas",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problemas_Proyectos_ProyectoId",
                table: "Problemas");
        }
    }
}
