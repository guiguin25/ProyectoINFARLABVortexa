namespace InfarlabSis.Application.DTOs.Inventario
{
    public class StockAlmacenDto
    {
        public int IdStockAlmacen { get; set; }
        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; } = string.Empty;
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int IdLote { get; set; }
        public string? NumeroLote { get; set; }
        public decimal CantidadActual { get; set; }
    }
}
