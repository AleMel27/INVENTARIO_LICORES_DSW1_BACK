namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Categoria
    {
        public long IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
