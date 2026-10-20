using AutoMapper;
using InfarlabSis.Application.DTOs.Despachos;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InfarlabSis.Infrastructure.Services
{
    public class DespachoService : IDespachoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventarioService _inventarioService;
        private readonly IMapper _mapper;

        public DespachoService(ApplicationDbContext context, IInventarioService inventarioService, IMapper mapper)
        {
            _context = context;
            _inventarioService = inventarioService;
            _mapper = mapper;
        }

        public async Task<EntregaDespachoDto> RegistrarEntregaParcialAsync(
            CrearEntregaDespachoDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.IdUsuario <= 0)
                throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
            if (dto.Items.Count == 0)
                throw new InvalidOperationException("La entrega debe contener al menos un producto.");

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.DetallesVenta)
                    .FirstOrDefaultAsync(v => v.IdVenta == dto.IdVenta && v.Estado, cancellationToken);
                if (venta is null)
                    throw new KeyNotFoundException("La venta no existe o está inactiva.");

                var cantidadesSolicitadas = dto.Items
                    .GroupBy(i => i.IdProducto)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.CantidadEntregada));
                var cantidadesPedidas = venta.DetallesVenta
                    .Where(d => d.Estado)
                    .GroupBy(d => d.IdProducto)
                    .ToDictionary(g => g.Key, g => (decimal)g.Sum(d => d.Cantidad));

                if (cantidadesPedidas.Count == 0)
                    throw new InvalidOperationException("La venta no tiene productos activos para despachar.");

                var entregasAnteriores = await _context.DetallesEntrega
                    .Where(d => d.Entrega.IdVenta == dto.IdVenta)
                    .Select(d => new { d.IdProducto, d.CantidadEntregada })
                    .ToListAsync(cancellationToken);
                var cantidadesEntregadas = entregasAnteriores
                    .GroupBy(d => d.IdProducto)
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.CantidadEntregada));

                foreach (var (idProducto, cantidadSolicitada) in cantidadesSolicitadas)
                {
                    if (!cantidadesPedidas.TryGetValue(idProducto, out var cantidadPedida))
                        throw new InvalidOperationException($"El producto {idProducto} no forma parte de la venta.");

                    cantidadesEntregadas.TryGetValue(idProducto, out var cantidadYaEntregada);
                    if (cantidadYaEntregada + cantidadSolicitada > cantidadPedida)
                        throw new InvalidOperationException($"La cantidad entregada del producto {idProducto} superaría la cantidad de la venta.");
                }

                foreach (var item in dto.Items)
                {
                    var loteValido = await _context.Lotes.AnyAsync(
                        l => l.IdLote == item.IdLote && l.IdProducto == item.IdProducto &&
                             l.Tipo != "Cuarentena" && l.Tipo != "Rechazado",
                        cancellationToken);
                    if (!loteValido)
                        throw new InvalidOperationException($"El lote {item.IdLote} no existe, no corresponde al producto o está en cuarentena/rechazado.");
                }

                var entrega = new EntregaDespacho
                {
                    IdVenta = dto.IdVenta,
                    IdUsuario = dto.IdUsuario,
                    NumeroNotaEntrega = dto.NumeroNotaEntrega.Trim(),
                    FechaEntrega = DateTime.UtcNow,
                    Observaciones = dto.Observaciones?.Trim(),
                    Detalles = dto.Items.Select(i => new DetalleEntrega
                    {
                        IdProducto = i.IdProducto,
                        IdLote = i.IdLote,
                        CantidadEntregada = i.CantidadEntregada
                    }).ToList()
                };

                _context.EntregasDespacho.Add(entrega);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var item in dto.Items)
                {
                    await _inventarioService.ActualizarStockYRegistrarKardexAsync(
                        item.IdAlmacen,
                        item.IdProducto,
                        item.IdLote,
                        -item.CantidadEntregada,
                        "SALIDA_PARCIAL",
                        $"ENTREGA:{entrega.IdEntrega};NOTA:{entrega.NumeroNotaEntrega}",
                        dto.IdUsuario,
                        cancellationToken);
                }

                foreach (var idProducto in cantidadesSolicitadas.Keys)
                {
                    cantidadesEntregadas.TryGetValue(idProducto, out var cantidadYaEntregada);
                    cantidadesEntregadas[idProducto] = cantidadYaEntregada + cantidadesSolicitadas[idProducto];
                }

                venta.EstadoEntrega = cantidadesPedidas.All(p =>
                    cantidadesEntregadas.TryGetValue(p.Key, out var entregada) && entregada >= p.Value)
                    ? "COMPLETADO"
                    : "PARCIAL";

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var entregaGuardada = await _context.EntregasDespacho
                    .AsNoTracking()
                    .Include(e => e.Detalles).ThenInclude(d => d.Producto)
                    .Include(e => e.Detalles).ThenInclude(d => d.Lote)
                    .FirstAsync(e => e.IdEntrega == entrega.IdEntrega, cancellationToken);
                return _mapper.Map<EntregaDespachoDto>(entregaGuardada);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<IReadOnlyList<EntregaDespachoDto>> ObtenerEntregasPorVentaAsync(
            int idVenta,
            CancellationToken cancellationToken = default)
        {
            var entregas = await _context.EntregasDespacho
                .AsNoTracking()
                .Where(e => e.IdVenta == idVenta)
                .Include(e => e.Detalles).ThenInclude(d => d.Producto)
                .Include(e => e.Detalles).ThenInclude(d => d.Lote)
                .OrderBy(e => e.FechaEntrega)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<EntregaDespachoDto>>(entregas);
        }
    }
}
