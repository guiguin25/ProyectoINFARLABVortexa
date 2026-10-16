using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfarlabSis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prueba7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVentaHistorials_VentaHistorials_HistorialIdHistorial",
                table: "DetalleVentaHistorials");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaHistorials_Usuarios_UsuarioModificoIdUsuario",
                table: "VentaHistorials");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaHistorials_Ventas_VentaIdVenta",
                table: "VentaHistorials");

            migrationBuilder.DropIndex(
                name: "IX_VentaHistorials_UsuarioModificoIdUsuario",
                table: "VentaHistorials");

            migrationBuilder.DropIndex(
                name: "IX_VentaHistorials_VentaIdVenta",
                table: "VentaHistorials");

            migrationBuilder.DropIndex(
                name: "IX_DetalleVentaHistorials_HistorialIdHistorial",
                table: "DetalleVentaHistorials");

            migrationBuilder.DropColumn(
                name: "UsuarioModificoIdUsuario",
                table: "VentaHistorials");

            migrationBuilder.DropColumn(
                name: "VentaIdVenta",
                table: "VentaHistorials");

            migrationBuilder.DropColumn(
                name: "HistorialIdHistorial",
                table: "DetalleVentaHistorials");

            migrationBuilder.AlterColumn<string>(
                name: "TipoCambio",
                table: "VentaHistorials",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "VentaHistorials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "DetalleVentaHistorials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Accion",
                table: "DetalleVentaHistorials",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_VentaHistorials_IdUsuarioModifico",
                table: "VentaHistorials",
                column: "IdUsuarioModifico");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaHistorials_Usuarios_IdUsuarioModifico",
                table: "VentaHistorials",
                column: "IdUsuarioModifico",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VentaHistorials_Usuarios_IdUsuarioModifico",
                table: "VentaHistorials");

            migrationBuilder.DropIndex(
                name: "IX_VentaHistorials_IdUsuarioModifico",
                table: "VentaHistorials");

            migrationBuilder.AlterColumn<string>(
                name: "TipoCambio",
                table: "VentaHistorials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "VentaHistorials",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificoIdUsuario",
                table: "VentaHistorials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VentaIdVenta",
                table: "VentaHistorials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "DetalleVentaHistorials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Accion",
                table: "DetalleVentaHistorials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "HistorialIdHistorial",
                table: "DetalleVentaHistorials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VentaHistorials_UsuarioModificoIdUsuario",
                table: "VentaHistorials",
                column: "UsuarioModificoIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_VentaHistorials_VentaIdVenta",
                table: "VentaHistorials",
                column: "VentaIdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentaHistorials_HistorialIdHistorial",
                table: "DetalleVentaHistorials",
                column: "HistorialIdHistorial");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVentaHistorials_VentaHistorials_HistorialIdHistorial",
                table: "DetalleVentaHistorials",
                column: "HistorialIdHistorial",
                principalTable: "VentaHistorials",
                principalColumn: "IdHistorial",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaHistorials_Usuarios_UsuarioModificoIdUsuario",
                table: "VentaHistorials",
                column: "UsuarioModificoIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaHistorials_Ventas_VentaIdVenta",
                table: "VentaHistorials",
                column: "VentaIdVenta",
                principalTable: "Ventas",
                principalColumn: "IdVenta",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
