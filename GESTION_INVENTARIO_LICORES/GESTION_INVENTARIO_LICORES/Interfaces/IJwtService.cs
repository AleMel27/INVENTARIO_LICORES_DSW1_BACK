namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        long idUsuario,
        string correo,
        string rol
    );
}
