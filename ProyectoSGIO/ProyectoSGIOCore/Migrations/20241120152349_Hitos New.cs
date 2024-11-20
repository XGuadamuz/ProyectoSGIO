using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class HitosNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Proyectos_ProyectoId",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "IdProyecto",
                table: "Hitos");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "Hitos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Proyectos_ProyectoId",
                table: "Hitos",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Proyectos_ProyectoId",
                table: "Hitos");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "Hitos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdProyecto",
                table: "Hitos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Proyectos_ProyectoId",
                table: "Hitos",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id");
        }
    }
}
