using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Producto
    {
        public int IdProducto { get; set; }
        // Nuevo/extendido esquema (mantener compatibilidad con código existente)
        public string Nombre { get; set; } = string.Empty; // nombre para compatibilidad
        public string? NombreGenerico { get; set; }
        public string? Codigo { get; set; } // código único del ítem
        public string? Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public int IdCategoria { get; set; }
        public int? IdProveedor { get; set; }
        public int? IdUnidad { get; set; } // FK a unidad_medida (no model existente actualmente)
        public string? RegistroSanitario { get; set; }
        public bool Estado { get; set; } = true;

        // Propiedades de navegación
        public virtual Categoria? Categoria { get; set; }
        public virtual Proveedor? Proveedor { get; set; }
        public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
    }
}
