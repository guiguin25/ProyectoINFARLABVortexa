using InfarlabSis.Application.DTOs.Devoluciones;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InfarlabSis.Infrastructure.Services
{
    public class DevolucionService : IDevolucionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventarioService _inventarioService;

        public DevolucionService(ApplicationDbContext context, IInventarioService inventarioService)
        {
            _context = context;
            _inventarioService = inventarioService;
        }

        public async Task<DevolucionReprocesoResultadoDto> RegistrarDevolucionConReprocesoAsync(
            DevolucionReprocesoDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.IdUsuario <= 0)
                throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
            if (dto.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad devuelta debe ser mayor que cero.");

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var venta = await _context.Ventas
                    .FirstOrDefaultAsync(v => v.IdVenta == dto.IdVenta && v.Estado, cancellationToken);
                if (venta is null)
                    throw new KeyNotFoundException("La venta no existe o está inactiva.");

                var loteOrigen = await _context.Lotes
                    .FirstOrDefaultAsync(l => l.IdLote == dto.IdLoteOrigen, cancellationToken);
                if (loteOrigen is null)
                    throw new KeyNotFoundException("El lote original no existe.");

                var productoEnVenta = await _context.DetalleVentas.AnyAsync(
                    d => d.IdVenta == dto.IdVenta && d.IdProducto == loteOrigen.IdProducto && d.Estado,
                    cancellationToken);
                if (!productoEnVenta)
                    throw new InvalidOperationException("El producto del lote no pertenece a la venta indicada.");

                var cantidadEntregada = await _context.DetallesEntrega
                    .Where(d => d.Entrega.IdVenta == dto.IdVenta &&
                                d.IdProducto == loteOrigen.IdProducto &&
                                d.IdLote == loteOrigen.IdLote)
                    .SumAsync(d => (decimal?)d.CantidadEntregada, cancellationToken) ?? 0m;

                var referenciaPrefijo = $"DEVOLUCION:{dto.IdVenta}:LOTE:{loteOrigen.IdLote}:";
                var cantidadDevueltaAnteriormente = await _context.Kardexs
                    .Where(k => k.TipoMovimiento == "DEVOLUCION_R" && k.Referencia.StartsWith(referenciaPrefijo))
                    .SumAsync(k => (decimal?)k.Cantidad, cancellationToken) ?? 0m;

                if (dto.Cantidad > cantidadEntregada - cantidadDevueltaAnteriormente)
                    throw new InvalidOperationException("La cantidad devuelta supera la cantidad entregada de ese lote en la venta.");

                var almacenCuarentena = await _context.Almacenes.FirstOrDefaultAsync(
                    a => EF.Functions.Like(a.TipoArea, "%Cuarentena%") || EF.Functions.Like(a.Nombre, "%Cuarentena%"),
                    cancellationToken);
                if (almacenCuarentena is null)
                    throw new InvalidOperationException("No existe un almacén configurado para Cuarentena.");

                var devolucion = new Devolucion
                {
                    IdVenta = dto.IdVenta,
                    IdCliente = venta.IdCliente,
                    FechaDevolucion = DateTime.UtcNow,
                    MotivoDevolucion = dto.Motivo.Trim(),
                    Estado = true,
                    DetallesDevolucion = new List<DetalleDevolucion>
                    {
                        new()
                        {
                            IdProducto = loteOrigen.IdProducto,
                            Cantidad = dto.Cantidad,
                            Estado = true
                        }
                    }
                };
                _context.Devoluciones.Add(devolucion);

                var loteReproceso = new Lote
                {
                    IdProducto = loteOrigen.IdProducto,
                    FechaElaboracion = loteOrigen.FechaElaboracion,
                    FechaVencimiento = loteOrigen.FechaVencimiento,
                    Tipo = "Cuarentena",
                    IdProveedor = loteOrigen.IdProveedor,
                    EsReproceso = true,
                    IdLoteOrigen = loteOrigen.IdLote
                };
                _context.Lotes.Add(loteReproceso);
                await _context.SaveChangesAsync(cancellationToken);

                var baseNumeroLote = string.IsNullOrWhiteSpace(loteOrigen.NumeroLote)
                    ? $"LOTE-{loteOrigen.IdLote}"
                    : loteOrigen.NumeroLote.Trim();
                loteReproceso.NumeroLote = $"{baseNumeroLote}-{loteReproceso.IdLote}R";
                await _context.SaveChangesAsync(cancellationToken);

                await _inventarioService.ActualizarStockYRegistrarKardexAsync(
                    almacenCuarentena.IdAlmacen,
                    loteOrigen.IdProducto,
                    loteReproceso.IdLote,
                    dto.Cantidad,
                    "DEVOLUCION_R",
                    $"{referenciaPrefijo}REPROCESO:{loteReproceso.IdLote};DEVOLUCION_ID:{devolucion.IdDevolucion}",
                    dto.IdUsuario,
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return new DevolucionReprocesoResultadoDto
                {
                    IdDevolucion = devolucion.IdDevolucion,
                    IdLoteReproceso = loteReproceso.IdLote,
                    NumeroLoteReproceso = loteReproceso.NumeroLote,
                    IdAlmacenCuarentena = almacenCuarentena.IdAlmacen,
                    Cantidad = dto.Cantidad
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
