using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly string? _conexion;

        public MarcaService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Marca> List()
        {
            List<Marca> lista = new List<Marca>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_marcas", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Marca
                            {
                                IdMarca = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                PaisOrigen = reader.IsDBNull(2) ? null : reader.GetString(2),
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

        public Marca GetMarca(long idMarca)
        {
            Marca marca = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_marca", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdMarca", idMarca);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            marca = new Marca
                            {
                                IdMarca = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                PaisOrigen = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3),
                                FechaCreacion = reader.GetDateTime(4),
                                FechaActualizacion = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return marca;
        }

        public bool Insert(Marca marca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_marca", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Nombre", marca.Nombre);
                        command.Parameters.AddWithValue("@PaisOrigen", (object)marca.PaisOrigen ?? DBNull.Value);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            marca.IdMarca = Convert.ToInt64(id);
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

        public bool Update(Marca marca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_marca", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdMarca", marca.IdMarca);
                        command.Parameters.AddWithValue("@Nombre", marca.Nombre);
                        command.Parameters.AddWithValue("@PaisOrigen", (object)marca.PaisOrigen ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", marca.Estado);

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

        public bool Delete(long idMarca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_marca", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdMarca", idMarca);

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