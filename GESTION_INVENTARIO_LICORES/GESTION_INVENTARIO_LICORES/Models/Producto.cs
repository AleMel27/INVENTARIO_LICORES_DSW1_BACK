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
        public decimal PrecioVenta { get; set; }
        public int StockMinimo { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
