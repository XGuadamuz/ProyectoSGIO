using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class HitosUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Usuario_UsuarioIdUsuario",
                table: "Hitos");

            migrationBuilder.DropIndex(
                name: "IX_Hitos_UsuarioIdUsuario",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Hitos");

            migrationBuilder.AlterColumn<int>(
                name: "IdUsuario",
                table: "Hitos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Hitos_IdUsuario",
                table: "Hitos",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Usuario_IdUsuario",
                table: "Hitos",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Usuario_IdUsuario",
                table: "Hitos");

            migrationBuilder.DropIndex(
                name: "IX_Hitos_IdUsuario",
                table: "Hitos");

            migrationBuilder.AlterColumn<int>(
                name: "IdUsuario",
                table: "Hitos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Hitos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hitos_UsuarioIdUsuario",
                table: "Hitos",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Usuario_UsuarioIdUsuario",
                table: "Hitos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }
    }
}
