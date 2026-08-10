using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IUsuarioService
    {

        List<Usuario> list();
        Usuario getUsuario(long idUsuario);
        bool insert(Usuario usuario);
        bool update(Usuario usuario);
        bool delete(long idUsuario);

    }
}
