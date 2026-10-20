using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Productos
{
    public class AreaCalidadDto
    {
        public int IdArea { get; set; }
        public int IdLote { get; set; }
        public string? NumeroLote { get; set; }
        public int IdAlmacenDestino { get; set; }
        public string? NombreAlmacenDestino { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public DateTime? FechaResultado { get; set; }
    }

    public class CrearAreaCalidadDto
    {
        [Range(1, int.MaxValue)]
        public int IdLote { get; set; }

        [Range(1, int.MaxValue)]
        public int IdAlmacenDestino { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public DateTime FechaEvaluacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaResultado { get; set; }
    }
}
