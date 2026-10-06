using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class AreaCalidad
    {
        public int IdArea { get; set; }
        public int IdLote { get; set; }
        public int IdAlmacenDestino { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public DateTime? FechaResultado { get; set; }

        // Propiedades de navegación
        public virtual Lote? Lote { get; set; }
        public virtual Almacen? AlmacenDestino { get; set; }
    }
}
