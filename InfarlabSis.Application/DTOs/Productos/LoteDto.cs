using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Productos
{
    public class LoteDto
    {
        public int IdLote { get; set; }
        public string? NumeroLote { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public DateTime FechaElaboracion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int? IdProveedor { get; set; }
        public string? NombreProveedor { get; set; }
        public bool EsReproceso { get; set; }
        public int? IdLoteOrigen { get; set; }
    }

    public class CrearLoteDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [StringLength(100)]
        public string? NumeroLote { get; set; }

        public DateTime FechaElaboracion { get; set; }
        public DateTime FechaVencimiento { get; set; }

        [Required, StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        public int? IdProveedor { get; set; }
    }
}
