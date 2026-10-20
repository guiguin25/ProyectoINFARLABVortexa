using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfarlabSis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloInventarioKardexYEntregas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Personas_IdPersona",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Personas_IdPersona",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_IdPersona",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_IdPersona",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "IdPersona",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "IdPersona",
                table: "Clientes");

            migrationBuilder.AddColumn<string>(
                name: "EstadoEntrega",
                table: "Ventas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmail",
                table: "Proveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoNombre",
                table: "Proveedores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoTelefono",
                table: "Proveedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Proveedores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Documento",
                table: "Proveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Proveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUnidad",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreGenerico",
                table: "Productos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistroSanitario",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StockMinimo",
                table: "Productos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsReproceso",
                table: "Lotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdLoteOrigen",
                table: "Lotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroLote",
                table: "Lotes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nit",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCliente",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntregasDespacho",
                columns: table => new
                {
                    IdEntrega = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    NumeroNotaEntrega = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasDespacho", x => x.IdEntrega);
                    table.ForeignKey(
                        name: "FK_EntregasDespacho_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntregasDespacho_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kardexs",
                columns: table => new
                {
                    IdKardex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdAlmacen = table.Column<int>(type: "int", nullable: false),
                    IdLote = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SaldoAnterior = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SaldoNuevo = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kardexs", x => x.IdKardex);
                    table.ForeignKey(
                        name: "FK_Kardexs_Almacenes_IdAlmacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacenes",
                        principalColumn: "IdAlmacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kardexs_Lotes_IdLote",
                        column: x => x.IdLote,
                        principalTable: "Lotes",
                        principalColumn: "IdLote",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kardexs_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kardexs_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAlmacenes",
                columns: table => new
                {
                    IdStockAlmacen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlmacen = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdLote = table.Column<int>(type: "int", nullable: false),
                    CantidadActual = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAlmacenes", x => x.IdStockAlmacen);
                    table.ForeignKey(
                        name: "FK_StockAlmacenes_Almacenes_IdAlmacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacenes",
                        principalColumn: "IdAlmacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAlmacenes_Lotes_IdLote",
                        column: x => x.IdLote,
                        principalTable: "Lotes",
                        principalColumn: "IdLote",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAlmacenes_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesEntrega",
                columns: table => new
                {
                    IdDetalleEntrega = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEntrega = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdLote = table.Column<int>(type: "int", nullable: false),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesEntrega", x => x.IdDetalleEntrega);
                    table.ForeignKey(
                        name: "FK_DetallesEntrega_EntregasDespacho_IdEntrega",
                        column: x => x.IdEntrega,
                        principalTable: "EntregasDespacho",
                        principalColumn: "IdEntrega",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesEntrega_Lotes_IdLote",
                        column: x => x.IdLote,
                        principalTable: "Lotes",
                        principalColumn: "IdLote",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesEntrega_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Codigo",
                table: "Productos",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_IdLoteOrigen",
                table: "Lotes",
                column: "IdLoteOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_NumeroLote",
                table: "Lotes",
                column: "NumeroLote",
                unique: true,
                filter: "[NumeroLote] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEntrega_IdEntrega",
                table: "DetallesEntrega",
                column: "IdEntrega");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEntrega_IdLote",
                table: "DetallesEntrega",
                column: "IdLote");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEntrega_IdProducto",
                table: "DetallesEntrega",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasDespacho_IdUsuario",
                table: "EntregasDespacho",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasDespacho_IdVenta",
                table: "EntregasDespacho",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Kardexs_IdAlmacen",
                table: "Kardexs",
                column: "IdAlmacen");

            migrationBuilder.CreateIndex(
                name: "IX_Kardexs_IdLote",
                table: "Kardexs",
                column: "IdLote");

            migrationBuilder.CreateIndex(
                name: "IX_Kardexs_IdProducto_IdAlmacen_FechaMovimiento",
                table: "Kardexs",
                columns: new[] { "IdProducto", "IdAlmacen", "FechaMovimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Kardexs_IdUsuario",
                table: "Kardexs",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlmacenes_IdAlmacen_IdProducto_IdLote",
                table: "StockAlmacenes",
                columns: new[] { "IdAlmacen", "IdProducto", "IdLote" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAlmacenes_IdLote",
                table: "StockAlmacenes",
                column: "IdLote");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlmacenes_IdProducto",
                table: "StockAlmacenes",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Lotes_IdLoteOrigen",
                table: "Lotes",
                column: "IdLoteOrigen",
                principalTable: "Lotes",
                principalColumn: "IdLote",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Lotes_IdLoteOrigen",
                table: "Lotes");

            migrationBuilder.DropTable(
                name: "DetallesEntrega");

            migrationBuilder.DropTable(
                name: "Kardexs");

            migrationBuilder.DropTable(
                name: "StockAlmacenes");

            migrationBuilder.DropTable(
                name: "EntregasDespacho");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Codigo",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Lotes_IdLoteOrigen",
                table: "Lotes");

            migrationBuilder.DropIndex(
                name: "IX_Lotes_NumeroLote",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "EstadoEntrega",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ContactoEmail",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ContactoNombre",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ContactoTelefono",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Documento",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "IdUnidad",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "NombreGenerico",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "RegistroSanitario",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "StockMinimo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EsReproceso",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "IdLoteOrigen",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "NumeroLote",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Nit",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TipoCliente",
                table: "Clientes");

            migrationBuilder.AddColumn<int>(
                name: "IdPersona",
                table: "Proveedores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPersona",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_IdPersona",
                table: "Proveedores",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdPersona",
                table: "Clientes",
                column: "IdPersona");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Personas_IdPersona",
                table: "Clientes",
                column: "IdPersona",
                principalTable: "Personas",
                principalColumn: "IdPersona",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Personas_IdPersona",
                table: "Proveedores",
                column: "IdPersona",
                principalTable: "Personas",
                principalColumn: "IdPersona",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
