namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Inventario
    {
        public long IdInventario { get; set; }

        public long IdProducto { get; set; }
        public long IdAlmacen { get; set; }

        public int StockActual { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
