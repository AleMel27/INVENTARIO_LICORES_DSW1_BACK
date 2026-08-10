using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IProductoService
    {
        List<Producto> list();
        Producto getProducto(long idProducto);
        bool insert(Producto producto);
        bool update(Producto producto);
        bool delete(long idProducto);
    }
}
