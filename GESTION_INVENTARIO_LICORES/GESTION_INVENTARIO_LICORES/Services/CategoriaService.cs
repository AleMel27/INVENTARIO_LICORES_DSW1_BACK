using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly string? _conexion;

        public CategoriaService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Categoria> List()
        {
            List<Categoria> lista = new List<Categoria>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_categorias", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Categoria
                            {
                                IdCategoria = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3),
                                FechaCreacion = reader.GetDateTime(4),
                                FechaActualizacion = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public Categoria GetCategoria(long idCategoria)
        {
            Categoria categoria = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_categoria", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCategoria", idCategoria);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new Categoria
                            {
                                IdCategoria = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3),
                                FechaCreacion = reader.GetDateTime(4),
                                FechaActualizacion = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return categoria;
        }
        public bool Insert(Categoria categoria)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_categoria", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            categoria.IdCategoria = Convert.ToInt64(id);
                            resp = true;
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }
        public bool Update(Categoria categoria)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_categoria", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCategoria", categoria.IdCategoria);
                        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", categoria.Estado);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }
        public bool Delete(long idCategoria)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_categoria", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }
    }
}