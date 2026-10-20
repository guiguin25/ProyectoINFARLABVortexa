namespace InfarlabSis.Application.DTOs.Inventario
{
    public class KardexDto
    {
        public int IdKardex { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; } = string.Empty;
        public int IdLote { get; set; }
        public string? NumeroLote { get; set; }
        public int IdUsuario { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
        public string Referencia { get; set; } = string.Empty;
        public DateTime FechaMovimiento { get; set; }
    }
}
