using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IDetalleCompraService
    {
        List<DetalleCompra> ListAll();
        List<DetalleCompra> ListByCompra(long idCompra);
        bool Insert(DetalleCompra detalle);
    }
}