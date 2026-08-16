namespace GESTION_INVENTARIO_LICORES.DTOs.Response
{
    public class LoginRespDto
    {
        public string Token { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public DateTime Expiracion { get; set; }

        public UsuarioRespDto Usuario { get; set; } = new();
    }
}
