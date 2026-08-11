using GESTION_INVENTARIO_LICORES.DTOs;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IInventarioService
    {
        List<Inventario> List();
        bool AjustarInventario(InventarioDto ajuste);
    }
}