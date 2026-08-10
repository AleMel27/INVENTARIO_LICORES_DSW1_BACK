using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IInventarioService
    {

        List<Inventario> list();
        Inventario getInventario(long idInventario);
        bool insert(Inventario inventario);
        bool update(Inventario inventario);
        bool delete(long idInventario);

    }
}
