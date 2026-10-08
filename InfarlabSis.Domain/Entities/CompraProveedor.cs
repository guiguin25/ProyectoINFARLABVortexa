using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class CompraProveedor
    {
        [Key]
        public int IdCompra { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} no puede exceder 150 caracteres")]
        public string ProductoComprado { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "datetime")]
        public DateTime FechaCompra { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Estado { get; set; } = true;

        // Relación: Una compra pertenece a un Proveedor
        [ForeignKey("IdProveedor")]
        public Proveedor Proveedor { get; set; }

        // Relación: Una compra está asociada a una Categoría
        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; }
    }
}