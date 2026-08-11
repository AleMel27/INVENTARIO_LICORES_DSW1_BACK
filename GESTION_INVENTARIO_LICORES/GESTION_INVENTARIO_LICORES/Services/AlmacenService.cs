using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class AlmacenService : IAlmacenService
    {
        private readonly string? _conexion;

        public AlmacenService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Almacen> List()
        {
            List<Almacen> lista = new List<Almacen>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_almacenes", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Almacen
                            {
                                IdAlmacen = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Ubicacion = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Estado = reader.GetBoolean(4),
                                FechaCreacion = reader.GetDateTime(5),
                                FechaActualizacion = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Almacen GetAlmacen(long idAlmacen)
        {
            Almacen almacen = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_almacen", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            almacen = new Almacen
                            {
                                IdAlmacen = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Ubicacion = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Estado = reader.GetBoolean(4),
                                FechaCreacion = reader.GetDateTime(5),
                                FechaActualizacion = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return almacen;
        }

        public bool Insert(Almacen almacen)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_almacen", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Nombre", almacen.Nombre);
                        command.Parameters.AddWithValue("@Ubicacion", almacen.Ubicacion);
                        command.Parameters.AddWithValue("@Descripcion", (object)almacen.Descripcion ?? DBNull.Value);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            almacen.IdAlmacen = Convert.ToInt64(id);
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

        public bool Update(Almacen almacen)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_almacen", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdAlmacen", almacen.IdAlmacen);
                        command.Parameters.AddWithValue("@Nombre", almacen.Nombre);
                        command.Parameters.AddWithValue("@Ubicacion", almacen.Ubicacion);
                        command.Parameters.AddWithValue("@Descripcion", (object)almacen.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", almacen.Estado);

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

        public bool Delete(long idAlmacen)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_almacen", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

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