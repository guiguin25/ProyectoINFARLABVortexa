using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class DetalleDevolucion
    {
        [Key]
        public int IdDetalleDevolucion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdDevolucion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Estado { get; set; } = true;

        // Relación: Un detalle pertenece a una Devolución
        [ForeignKey("IdDevolucion")]
        public Devolucion Devolucion { get; set; }

        // Relación: Un detalle corresponde a un Producto
        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}