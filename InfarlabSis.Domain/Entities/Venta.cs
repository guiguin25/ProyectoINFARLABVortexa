using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "datetime")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder 50 caracteres")]
        public string TipoPago { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Estado { get; set; } = true;

        public string EstadoEntrega { get; set; } = "PENDIENTE";

        // Relación: Una venta es realizada por un Usuario
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        // Relación: Una venta es realizada a un Cliente
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        // Relación: Una venta tiene muchos Detalles
        public List<DetalleVenta> DetallesVenta { get; set; }

        // Relación: Una venta puede tener una o más Devoluciones
        public List<Devolucion> Devoluciones { get; set; }

        public virtual ICollection<EntregaDespacho> Entregas { get; set; } = new List<EntregaDespacho>();

        public Venta()
        {
            DetallesVenta = new List<DetalleVenta>();
        }
    }
}