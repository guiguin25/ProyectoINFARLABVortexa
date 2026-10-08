using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Devolucion
    {
        [Key]
        public int IdDevolucion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdVenta { get; set; }

        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "datetime")]
        public DateTime FechaDevolucion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(250, ErrorMessage = "El campo {0} no puede exceder 250 caracteres")]
        public string MotivoDevolucion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Estado { get; set; } = true;

        // Relación: Una devolución pertenece a una Venta
        [ForeignKey("IdVenta")]
        public Venta Venta { get; set; }

        // Relación: Una devolución puede estar asociada a un Cliente
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        // Relación: Una devolución tiene muchos Detalles
        public List<DetalleDevolucion> DetallesDevolucion { get; set; }
    }
}