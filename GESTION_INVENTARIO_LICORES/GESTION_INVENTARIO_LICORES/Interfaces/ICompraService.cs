using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface ICompraService
    {
        List<Compra> list();
        Compra getCompra(long idCompra);
        bool insert(Compra compra);
        bool update(Compra compra);
        bool delete(long idCompra);

    }
}
