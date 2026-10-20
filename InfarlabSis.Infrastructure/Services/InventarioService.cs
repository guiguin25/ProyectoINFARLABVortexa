using AutoMapper;
using InfarlabSis.Application.DTOs.Inventario;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InfarlabSis.Infrastructure.Services
{
    public class InventarioService : IInventarioService
    {
        private static readonly HashSet<string> TiposMovimiento = new(StringComparer.OrdinalIgnoreCase)
        {
            "ENTRADA", "SALIDA_PARCIAL", "DEVOLUCION_R", "AJUSTE"
        };

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public InventarioService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StockAlmacenDto> ActualizarStockYRegistrarKardexAsync(
            int idAlmacen,
            int idProducto,
            int idLote,
            decimal cantidadCambio,
            string tipoMovimiento,
            string referencia,
            int idUsuario,
            CancellationToken cancellationToken = default)
        {
            if (cantidadCambio == 0)
                throw new InvalidOperationException("El cambio de stock no puede ser cero.");
            if (string.IsNullOrWhiteSpace(tipoMovimiento) || !TiposMovimiento.Contains(tipoMovimiento))
                throw new InvalidOperationException("El tipo de movimiento no es válido.");
            if (string.IsNullOrWhiteSpace(referencia))
                throw new InvalidOperationException("La referencia del movimiento es obligatoria.");
            if ((tipoMovimiento.Equals("ENTRADA", StringComparison.OrdinalIgnoreCase) ||
                 tipoMovimiento.Equals("DEVOLUCION_R", StringComparison.OrdinalIgnoreCase)) && cantidadCambio < 0)
                throw new InvalidOperationException("Este tipo de movimiento requiere una cantidad positiva.");
            if (tipoMovimiento.Equals("SALIDA_PARCIAL", StringComparison.OrdinalIgnoreCase) && cantidadCambio > 0)
                throw new InvalidOperationException("Una salida parcial requiere una cantidad negativa.");

            var ownsTransaction = _context.Database.CurrentTransaction is null;
            await using var transaction = ownsTransaction
                ? await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
                : null;

            try
            {
                var loteValido = await _context.Lotes.AnyAsync(
                    l => l.IdLote == idLote && l.IdProducto == idProducto,
                    cancellationToken);
                if (!loteValido)
                    throw new InvalidOperationException("El lote no existe o no corresponde al producto indicado.");

                var stock = await _context.StockAlmacenes.SingleOrDefaultAsync(
                    s => s.IdAlmacen == idAlmacen && s.IdProducto == idProducto && s.IdLote == idLote,
                    cancellationToken);

                if (stock is null)
                {
                    if (cantidadCambio < 0)
                        throw new InvalidOperationException("No existe stock disponible para realizar la salida.");

                    stock = new StockAlmacen
                    {
                        IdAlmacen = idAlmacen,
                        IdProducto = idProducto,
                        IdLote = idLote,
                        CantidadActual = 0
                    };
                    _context.StockAlmacenes.Add(stock);
                }

                var saldoAnterior = stock.CantidadActual;
                var saldoNuevo = saldoAnterior + cantidadCambio;
                if (saldoNuevo < 0)
                    throw new InvalidOperationException("El stock disponible es insuficiente.");

                stock.CantidadActual = saldoNuevo;
                _context.Kardexs.Add(new Kardex
                {
                    IdProducto = idProducto,
                    IdAlmacen = idAlmacen,
                    IdLote = idLote,
                    IdUsuario = idUsuario,
                    TipoMovimiento = tipoMovimiento.ToUpperInvariant(),
                    Cantidad = Math.Abs(cantidadCambio),
                    SaldoAnterior = saldoAnterior,
                    SaldoNuevo = saldoNuevo,
                    Referencia = referencia.Trim(),
                    FechaMovimiento = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);
                if (transaction is not null)
                    await transaction.CommitAsync(cancellationToken);

                await _context.Entry(stock).Reference(s => s.Almacen).LoadAsync(cancellationToken);
                await _context.Entry(stock).Reference(s => s.Producto).LoadAsync(cancellationToken);
                await _context.Entry(stock).Reference(s => s.Lote).LoadAsync(cancellationToken);
                return _mapper.Map<StockAlmacenDto>(stock);
            }
            catch
            {
                if (transaction is not null)
                    await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<IReadOnlyList<StockAlmacenDto>> ObtenerStockPorAlmacenAsync(
            int? idAlmacen = null,
            int? idProducto = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.StockAlmacenes
                .AsNoTracking()
                .Include(s => s.Almacen)
                .Include(s => s.Producto)
                .Include(s => s.Lote)
                .AsQueryable();

            if (idAlmacen.HasValue)
                query = query.Where(s => s.IdAlmacen == idAlmacen.Value);
            if (idProducto.HasValue)
                query = query.Where(s => s.IdProducto == idProducto.Value);

            var stock = await query.OrderBy(s => s.IdAlmacen).ThenBy(s => s.IdProducto).ThenBy(s => s.IdLote)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<StockAlmacenDto>>(stock);
        }

        public async Task<IReadOnlyList<KardexDto>> ObtenerHistorialKardexAsync(
            int? idProducto = null,
            int? idAlmacen = null,
            int? idLote = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            int limite = 200,
            CancellationToken cancellationToken = default)
        {
            if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                throw new InvalidOperationException("La fecha inicial no puede ser posterior a la fecha final.");

            limite = Math.Clamp(limite, 1, 1000);
            var query = _context.Kardexs
                .AsNoTracking()
                .Include(k => k.Producto)
                .Include(k => k.Almacen)
                .Include(k => k.Lote)
                .AsQueryable();

            if (idProducto.HasValue)
                query = query.Where(k => k.IdProducto == idProducto.Value);
            if (idAlmacen.HasValue)
                query = query.Where(k => k.IdAlmacen == idAlmacen.Value);
            if (idLote.HasValue)
                query = query.Where(k => k.IdLote == idLote.Value);
            if (desde.HasValue)
                query = query.Where(k => k.FechaMovimiento >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(k => k.FechaMovimiento <= hasta.Value);

            var kardex = await query.OrderByDescending(k => k.FechaMovimiento).Take(limite).ToListAsync(cancellationToken);
            return _mapper.Map<List<KardexDto>>(kardex);
        }
    }
}
