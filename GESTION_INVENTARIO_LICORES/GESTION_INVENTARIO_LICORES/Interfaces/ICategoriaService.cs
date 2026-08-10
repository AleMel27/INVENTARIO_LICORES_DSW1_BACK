using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface ICategoriaService
    {
        List<Categoria> list();
        Categoria getCategoria(long idCategoria);
        bool insert(Categoria categoria);
        bool update(Categoria categoria);
        bool delete(long idCategoria);
    }
}