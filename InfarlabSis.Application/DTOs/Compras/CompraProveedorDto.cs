using System;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Compras
{
    public class CompraProveedorDto
    {
        public int IdCompra { get; set; }

        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El producto comprado es obligatorio.")]
        [StringLength(150)]
        public string ProductoComprado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        public DateTime FechaCompra { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser mayor o igual a cero.")]
        public decimal Total { get; set; }

        //Propiedades de navegación
        public string? NombreProveedor { get; set; }
        public string? NombreCategoria { get; set; }
    }
}