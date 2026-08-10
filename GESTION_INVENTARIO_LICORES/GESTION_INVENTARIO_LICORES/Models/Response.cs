namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Response<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}