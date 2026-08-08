namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Marca
    {
        public long IdMarca { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? PaisOrigen { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

    }
}
