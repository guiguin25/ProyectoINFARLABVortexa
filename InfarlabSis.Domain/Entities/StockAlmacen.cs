namespace InfarlabSis.Domain.Entities
{
    public class StockAlmacen
    {
        public int IdStockAlmacen { get; set; }
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public int IdLote { get; set; }
        public decimal CantidadActual { get; set; }

        public virtual Almacen Almacen { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
        public virtual Lote Lote { get; set; } = null!;
    }
}
