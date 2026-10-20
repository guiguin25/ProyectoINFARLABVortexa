using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InfarlabSis.Application.DTOs.Devoluciones
{
    public class DevolucionReprocesoDto
    {
        [Range(1, int.MaxValue)]
        public int IdVenta { get; set; }

        [Range(1, int.MaxValue)]
        public int IdLoteOrigen { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required, StringLength(250)]
        public string Motivo { get; set; } = string.Empty;

        [JsonIgnore]
        public int IdUsuario { get; set; }
    }
}
