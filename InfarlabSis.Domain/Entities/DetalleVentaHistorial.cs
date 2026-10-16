using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class DetalleVentaHistorial
    {
        [Key]
        public int IdDetalleHistorial { get; set; }

        public int IdHistorial { get; set; }

        public int IdProducto { get; set; }

        [StringLength(200)]
        public string NombreProducto { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;
    }
}
