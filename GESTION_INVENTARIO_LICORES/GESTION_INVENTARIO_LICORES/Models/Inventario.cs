namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Inventario
    {
        public long IdInventario { get; set; }
        public long IdProducto { get; set; }
        public long IdAlmacen { get; set; }
        public int StockActual { get; set; } = 0;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Propiedades opcionales de navegación (para incluir datos del Producto o Almacén en las consultas)
        public Producto? Producto { get; set; }
        public Almacen? Almacen { get; set; }
    }
}
