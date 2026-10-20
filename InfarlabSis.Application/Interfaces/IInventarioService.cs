using InfarlabSis.Application.DTOs.Inventario;

namespace InfarlabSis.Application.Interfaces
{
    public interface IInventarioService
    {
        Task<StockAlmacenDto> ActualizarStockYRegistrarKardexAsync(
            int idAlmacen,
            int idProducto,
            int idLote,
            decimal cantidadCambio,
            string tipoMovimiento,
            string referencia,
            int idUsuario,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<StockAlmacenDto>> ObtenerStockPorAlmacenAsync(
            int? idAlmacen = null,
            int? idProducto = null,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<KardexDto>> ObtenerHistorialKardexAsync(
            int? idProducto = null,
            int? idAlmacen = null,
            int? idLote = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            int limite = 200,
            CancellationToken cancellationToken = default);
    }
}
