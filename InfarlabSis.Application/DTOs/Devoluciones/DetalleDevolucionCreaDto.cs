using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Devoluciones
{
    public class DetalleDevolucionCreaDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }
    }
}