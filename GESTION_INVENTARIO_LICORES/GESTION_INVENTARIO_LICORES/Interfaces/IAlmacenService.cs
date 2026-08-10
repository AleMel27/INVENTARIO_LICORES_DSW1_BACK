using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IAlmacenService
    {
        List<Almacen> list();
        Almacen getAlmacen(long idAlmacen);
        bool insert(Almacen almacen);
        bool update(Almacen almacen);
        bool delete(long idAlmacen);
    }
}
