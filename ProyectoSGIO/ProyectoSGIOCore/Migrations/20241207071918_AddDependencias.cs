using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoSGIOCore.Migrations
{
    /// <inheritdoc />
    public partial class AddDependencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dependencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TareaPredecesoraId = table.Column<int>(type: "int", nullable: false),
                    TareaSucesoraId = table.Column<int>(type: "int", nullable: false),
                    TipoDependencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependencias_Tareas_TareaPredecesoraId",
                        column: x => x.TareaPredecesoraId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dependencias_Tareas_TareaSucesoraId",
                        column: x => x.TareaSucesoraId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dependencias_TareaPredecesoraId",
                table: "Dependencias",
                column: "TareaPredecesoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencias_TareaSucesoraId",
                table: "Dependencias",
                column: "TareaSucesoraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependencias");
        }
    }
}
