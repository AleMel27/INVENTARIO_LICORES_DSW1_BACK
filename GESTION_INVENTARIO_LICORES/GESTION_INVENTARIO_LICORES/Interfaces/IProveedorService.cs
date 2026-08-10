using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IProveedorService
    {
        List<Proveedor> list();
        Proveedor getProveedor(long idProveedor);
        bool insert(Proveedor proveedor);
        bool update(Proveedor proveedor);
        bool delete(long idProveedor);
    }
}
