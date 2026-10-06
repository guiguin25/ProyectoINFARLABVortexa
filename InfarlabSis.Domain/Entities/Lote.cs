using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Lote
    {
        public int IdLote { get; set; }
        public int IdProducto { get; set; }
        public DateTime FechaElaboracion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Tipo { get; set; } = string.Empty; // Ej: Cuarentena, Aprobado, Rechazado
        public int? IdProveedor { get; set; }

        // Propiedades de navegación
        public virtual Producto? Producto { get; set; }
        public virtual Proveedor? Proveedor { get; set; }
        public virtual ICollection<AreaCalidad> EvaluacionesCalidad { get; set; } = new List<AreaCalidad>();
    }
}
