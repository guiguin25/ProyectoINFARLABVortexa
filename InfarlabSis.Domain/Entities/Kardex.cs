namespace InfarlabSis.Domain.Entities
{
    public class Kardex
    {
        public int IdKardex { get; set; }
        public int IdProducto { get; set; }
        public int IdAlmacen { get; set; }
        public int IdLote { get; set; }
        public int IdUsuario { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
        public string Referencia { get; set; } = string.Empty;
        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

        public virtual Producto Producto { get; set; } = null!;
        public virtual Almacen Almacen { get; set; } = null!;
        public virtual Lote Lote { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
