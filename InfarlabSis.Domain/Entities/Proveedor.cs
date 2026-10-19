using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        // Datos del proveedor (se separa de Persona)
        public string Nombre { get; set; } = string.Empty;
        public string? Documento { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Datos de contacto dentro del proveedor
        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
    }
}
