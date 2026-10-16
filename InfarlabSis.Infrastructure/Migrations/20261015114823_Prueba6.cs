using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfarlabSis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prueba6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VentaHistorials",
                columns: table => new
                {
                    IdHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioModifico = table.Column<int>(type: "int", nullable: false),
                    TipoCambio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatosAntes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatosDespues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VentaIdVenta = table.Column<int>(type: "int", nullable: false),
                    UsuarioModificoIdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaHistorials", x => x.IdHistorial);
                    table.ForeignKey(
                        name: "FK_VentaHistorials_Usuarios_UsuarioModificoIdUsuario",
                        column: x => x.UsuarioModificoIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VentaHistorials_Ventas_VentaIdVenta",
                        column: x => x.VentaIdVenta,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleVentaHistorials",
                columns: table => new
                {
                    IdDetalleHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHistorial = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    NombreProducto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HistorialIdHistorial = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVentaHistorials", x => x.IdDetalleHistorial);
                    table.ForeignKey(
                        name: "FK_DetalleVentaHistorials_VentaHistorials_HistorialIdHistorial",
                        column: x => x.HistorialIdHistorial,
                        principalTable: "VentaHistorials",
                        principalColumn: "IdHistorial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentaHistorials_HistorialIdHistorial",
                table: "DetalleVentaHistorials",
                column: "HistorialIdHistorial");

            migrationBuilder.CreateIndex(
                name: "IX_VentaHistorials_UsuarioModificoIdUsuario",
                table: "VentaHistorials",
                column: "UsuarioModificoIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_VentaHistorials_VentaIdVenta",
                table: "VentaHistorials",
                column: "VentaIdVenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleVentaHistorials");

            migrationBuilder.DropTable(
                name: "VentaHistorials");
        }
    }
}
