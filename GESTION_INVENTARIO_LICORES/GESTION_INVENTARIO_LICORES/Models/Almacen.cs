namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Almacen
    {

        public long IdAlmacen { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

    }
}
