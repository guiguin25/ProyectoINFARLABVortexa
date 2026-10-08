using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class DetalleVenta
    {
        [Key]
        public int IdDetalleVenta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Estado { get; set; } = true;

        // Relación: Un detalle pertenece a una Venta
        [ForeignKey("IdVenta")]
        public Venta Venta { get; set; }

        // Relación: Un detalle corresponde a un Producto
        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}