using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface ICategoriaService
    {
        List<Categoria> List();
        Categoria GetCategoria(long idCategoria);
        bool Insert(Categoria categoria);
        bool Update(Categoria categoria);
        bool Delete(long idCategoria);
    }
}