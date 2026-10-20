using InfarlabSis.Application.DTOs.Devoluciones;

namespace InfarlabSis.Application.Interfaces
{
    public interface IDevolucionService
    {
        Task<DevolucionReprocesoResultadoDto> RegistrarDevolucionConReprocesoAsync(
            DevolucionReprocesoDto dto,
            CancellationToken cancellationToken = default);
    }
}
