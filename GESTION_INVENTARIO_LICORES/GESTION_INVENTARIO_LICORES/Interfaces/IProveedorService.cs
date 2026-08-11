using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IProveedorService
    {   
        List<Proveedor> List();
        Proveedor GetProveedor(long idProveedor);
        bool Insert(Proveedor proveedor);
        bool Update(Proveedor proveedor);
        bool Delete(long idProveedor);
    }
}