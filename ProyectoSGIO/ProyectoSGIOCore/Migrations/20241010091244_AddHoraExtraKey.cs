using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class AddHoraExtraKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Usuario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Usuario",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasExtraAcumuladas",
                table: "Empleado",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UmbralHorasExtra",
                table: "Empleado",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "HorasExtra",
                columns: table => new
                {
                    IdHoraExtra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadHoras = table.Column<double>(type: "float", nullable: false),
                    Aprobada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorasExtra", x => x.IdHoraExtra);
                    table.ForeignKey(
                        name: "FK_HorasExtra_Empleado_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "IdUsuario",
                keyValue: 1,
                columns: new[] { "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Id", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 0, "f506d0d1-1bc9-409f-8f65-d5590b27e244", null, false, "3a57c7d8-0e06-461d-b6b7-bef287447572", false, null, null, null, null, null, false, "9cf8c15a-2173-4131-8359-cfb5000590a4", false, null });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "IdUsuario",
                keyValue: 2,
                columns: new[] { "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Id", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 0, "2c535415-8da8-4764-b2cf-293e6dd72b01", null, false, "36ed81c3-e82e-4006-a480-c29414a918e3", false, null, null, null, null, null, false, "37ed27f8-2206-40a4-b64f-62b4e62c6386", false, null });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "IdUsuario",
                keyValue: 3,
                columns: new[] { "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Id", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 0, "8befc451-e30c-4e2a-8302-54e8aaa82c8b", null, false, "8663f25d-5068-41db-88bd-fd1ef7075c15", false, null, null, null, null, null, false, "ea7efbdf-ea7b-4158-b3ab-510c754f939e", false, null });

            migrationBuilder.CreateIndex(
                name: "IX_HorasExtra_IdEmpleado",
                table: "HorasExtra",
                column: "IdEmpleado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorasExtra");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "HorasExtraAcumuladas",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "UmbralHorasExtra",
                table: "Empleado");
        }
    }
}
