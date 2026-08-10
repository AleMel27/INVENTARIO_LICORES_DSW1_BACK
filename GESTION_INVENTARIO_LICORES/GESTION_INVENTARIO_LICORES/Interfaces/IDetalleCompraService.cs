using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IDetalleCompraService
    {


        List<DetalleCompra> list();
        DetalleCompra getDetalleCompra(long idDetalleCompra);
        bool insert(DetalleCompra detalleCompra);
        bool update(DetalleCompra detalleCompra);
        bool delete(long idDetalleCompra);


    }
}
