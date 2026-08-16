namespace GESTION_INVENTARIO_LICORES.Models
{
    public class MovimientoInventario
    {
        public long IdMovimiento { get; set; }
        public long IdProducto { get; set; }
        public long IdAlmacen { get; set; }
        public long IdUsuario { get; set; }

        public long? IdCompra { get; set; }

        public long IdTipoMovimiento { get; set; }

        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockPosterior { get; set; }

        public string Motivo { get; set; } = string.Empty;
        public string? Referencia { get; set; }

        public DateTime FechaMovimiento { get; set; }
    }
}
