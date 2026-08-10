using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IMovimientoInventarioService
    {

        List<MovimientoInventario> list();
        MovimientoInventario getMovimientoInventario(long idMovimiento);
        bool insert(MovimientoInventario movimiento);
        bool update(MovimientoInventario movimiento);
        bool delete(long idMovimiento);

    }
}
