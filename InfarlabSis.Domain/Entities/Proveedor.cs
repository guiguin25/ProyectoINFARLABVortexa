using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public int? IdPersona { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Persona? Persona { get; set; }
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
    }
}
