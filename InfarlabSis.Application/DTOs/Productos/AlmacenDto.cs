using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfarlabSis.Application.DTOs.Productos
{
    public class AlmacenDto
    {
        public int IdAlmacen { get; set; }

        [Required(ErrorMessage = "El nombre del almacén es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de área es obligatorio.")]
        [StringLength(50)]
        public string TipoArea { get; set; } = string.Empty; // Ej: Materia Prima, Cuarentena, Producto Terminado, Rechazados

        public bool Estado { get; set; } = true;
    }
}
