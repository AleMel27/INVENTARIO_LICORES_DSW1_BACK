using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IMarcaService
    {
        List<Marca> list();
        Marca getMarca(long idMarca);
        bool insert(Marca marca);
        bool update(Marca marca);
        bool delete(long idMarca);
    }
}