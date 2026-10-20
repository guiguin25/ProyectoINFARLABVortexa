using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Productos
{
    public class UnidadMedidaDto
    {
        public int IdUnidad { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}
