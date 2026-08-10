using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly string? conexion;

        public MarcaService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion");
        }

        public List<Marca> list()
        {
            List<Marca> temporal = new List<Marca>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_marcas", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Marca marca = new Marca
                            {
                                IdMarca = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                PaisOrigen = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3),
                                FechaCreacion = reader.GetDateTime(4),
                                FechaActualizacion = reader.GetDateTime(5)
                            };
                            temporal.Add(marca);
                        }
                    }
                }
            }
            return temporal;
        }

        public Marca getMarca(long idMarca)
        {
            Marca? marca = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_marca_by_id", con))
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

        public bool insert(Marca marca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
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
                        command.Parameters.AddWithValue("@PaisOrigen", (object?)marca.PaisOrigen ?? DBNull.Value);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("ERROR SQL: " + ex.Message);
                }
            }
            return resp;
        }

        public bool update(Marca marca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
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
                        command.Parameters.AddWithValue("@PaisOrigen", (object?)marca.PaisOrigen ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", marca.Estado);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return resp;
        }

        public bool delete(long idMarca)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
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
                }
            }
            return resp;
        }
    }
}