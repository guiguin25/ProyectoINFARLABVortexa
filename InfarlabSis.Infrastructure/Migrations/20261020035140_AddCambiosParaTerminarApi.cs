using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfarlabSis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCambiosParaTerminarApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    IdUnidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.IdUnidad);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdUnidad",
                table: "Productos",
                column: "IdUnidad");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Nombre",
                table: "UnidadesMedida",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_IdUnidad",
                table: "Productos",
                column: "IdUnidad",
                principalTable: "UnidadesMedida",
                principalColumn: "IdUnidad",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_IdUnidad",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_Productos_IdUnidad",
                table: "Productos");
        }
    }
}
