using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IUsuarioService
    {
        List<Usuario> List();
        Usuario GetUsuario(long idUsuario);
        bool Insert(Usuario usuario);
        bool Update(Usuario usuario);
        bool Delete(long idUsuario);
        bool ChangePassword(long idUsuario, string nuevoPasswordHash); // <- Nuevo método
    }
}