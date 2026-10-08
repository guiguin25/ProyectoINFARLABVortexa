using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class DetalleVentaCreaDto
    {
        [Required(ErrorMessage = "El ID de la venta es obligatorio.")]
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a cero.")]
        public decimal PrecioUnitario { get; set; }
    }
}