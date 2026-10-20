using InfarlabSis.Application.DTOs.Despachos;

namespace InfarlabSis.Application.Interfaces
{
    public interface IDespachoService
    {
        Task<EntregaDespachoDto> RegistrarEntregaParcialAsync(CrearEntregaDespachoDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EntregaDespachoDto>> ObtenerEntregasPorVentaAsync(int idVenta, CancellationToken cancellationToken = default);
    }
}
