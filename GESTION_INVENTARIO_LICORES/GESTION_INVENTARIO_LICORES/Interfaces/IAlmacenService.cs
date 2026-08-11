using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IAlmacenService
    {
        List<Almacen> List();
        Almacen GetAlmacen(long idAlmacen);
        bool Insert(Almacen almacen);
        bool Update(Almacen almacen);
        bool Delete(long idAlmacen);
    }
}