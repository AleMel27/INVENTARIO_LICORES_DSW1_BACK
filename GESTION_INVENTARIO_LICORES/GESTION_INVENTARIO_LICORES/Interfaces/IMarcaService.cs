using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IMarcaService
    {
        List<Marca> List();
        Marca GetMarca(long idMarca);
        bool Insert(Marca marca);
        bool Update(Marca marca);
        bool Delete(long idMarca);
    }
}