using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InfarlabSis.Application.DTOs.Despachos
{
    public class CrearEntregaDespachoDto
    {
        [Required]
        public int IdVenta { get; set; }

        [Required, StringLength(100)]
        public string NumeroNotaEntrega { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Required, MinLength(1)]
        public List<DetalleEntregaSolicitudDto> Items { get; set; } = new();

        [JsonIgnore]
        public int IdUsuario { get; set; }
    }

    public class DetalleEntregaSolicitudDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int IdAlmacen { get; set; }

        [Range(1, int.MaxValue)]
        public int IdLote { get; set; }

        [Range(typeof(decimal), "0.001", "999999999999")]
        public decimal CantidadEntregada { get; set; }
    }
}
