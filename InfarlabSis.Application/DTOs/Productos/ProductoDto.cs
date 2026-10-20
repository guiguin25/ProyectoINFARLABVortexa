using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfarlabSis.Application.DTOs.Productos
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150)]
        public string? NombreGenerico { get; set; }

        [StringLength(50)]
        public string? Codigo { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "El precio de venta debe ser un número positivo.")]
        public decimal PrecioVenta { get; set; }

        public int StockActual { get; set; }

        public decimal StockMinimo { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int IdCategoria { get; set; }

        public int? IdProveedor { get; set; }

        public int? IdUnidad { get; set; }

        public string? NombreUnidad { get; set; }

        public string? RegistroSanitario { get; set; }

        public bool Estado { get; set; } = true;

        // Campos informativos para lectura (GET)
        public string? NombreCategoria { get; set; }
        public string? NombreProveedor { get; set; }
    }
}
