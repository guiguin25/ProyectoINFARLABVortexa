using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string TipoArea { get; set; } = string.Empty; // Ej: Materia Prima, Cuarentena, Producto Terminado, Rechazados
        public bool Estado { get; set; } = true;
    }
}
