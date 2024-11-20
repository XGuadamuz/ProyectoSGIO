using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class HitosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Empleado_ResponsableIdEmpleado",
                table: "Hitos");

            migrationBuilder.RenameColumn(
                name: "ResponsableIdEmpleado",
                table: "Hitos",
                newName: "UsuarioIdUsuario");

            migrationBuilder.RenameColumn(
                name: "IdEmpleado",
                table: "Hitos",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Hitos_ResponsableIdEmpleado",
                table: "Hitos",
                newName: "IX_Hitos_UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Usuario_UsuarioIdUsuario",
                table: "Hitos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitos_Usuario_UsuarioIdUsuario",
                table: "Hitos");

            migrationBuilder.RenameColumn(
                name: "UsuarioIdUsuario",
                table: "Hitos",
                newName: "ResponsableIdEmpleado");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Hitos",
                newName: "IdEmpleado");

            migrationBuilder.RenameIndex(
                name: "IX_Hitos_UsuarioIdUsuario",
                table: "Hitos",
                newName: "IX_Hitos_ResponsableIdEmpleado");

            migrationBuilder.AddForeignKey(
                name: "FK_Hitos_Empleado_ResponsableIdEmpleado",
                table: "Hitos",
                column: "ResponsableIdEmpleado",
                principalTable: "Empleado",
                principalColumn: "IdEmpleado");
        }
    }
}
