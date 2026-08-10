namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Producto
    {
        public long IdProducto { get; set; }
        public long IdCategoria { get; set; }
        public long IdMarca { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CapacidadMl { get; set; }
        public decimal GradoAlcoholico { get; set; }
        public decimal PrecioVenta { get; set; } = 0.00m;
        public int StockMinimo { get; set; } = 0;
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Propiedades opcionales de navegación (por si las utilizas para los JOINs)
        public Categoria? Categoria { get; set; }
        public Marca? Marca { get; set; }
    }
}
