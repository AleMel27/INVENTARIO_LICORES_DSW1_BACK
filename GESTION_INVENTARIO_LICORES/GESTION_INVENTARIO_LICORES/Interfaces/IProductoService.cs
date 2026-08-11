using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IProductoService
    {
        List<Producto> List();
        Producto GetProducto(long idProducto);
        bool Insert(Producto producto);
        bool Update(Producto producto);
        bool Delete(long idProducto);
    }
}