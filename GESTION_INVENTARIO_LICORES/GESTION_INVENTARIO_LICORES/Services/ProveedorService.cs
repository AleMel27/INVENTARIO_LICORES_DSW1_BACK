using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly string? _conexion;

        public ProveedorService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Proveedor> List()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_proveedores", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                IdProveedor = reader.GetInt64(0),
                                Ruc = reader.GetString(1),
                                RazonSocial = reader.GetString(2),
                                Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Correo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Estado = reader.GetBoolean(6),
                                FechaCreacion = reader.GetDateTime(7),
                                FechaActualizacion = reader.GetDateTime(8)
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public Proveedor GetProveedor(long idProveedor)
        {
            Proveedor proveedor = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_proveedor", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            proveedor = new Proveedor
                            {
                                IdProveedor = reader.GetInt64(0),
                                Ruc = reader.GetString(1),
                                RazonSocial = reader.GetString(2),
                                Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Correo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Estado = reader.GetBoolean(6),
                                FechaCreacion = reader.GetDateTime(7),
                                FechaActualizacion = reader.GetDateTime(8)
                            };
                        }
                    }
                }
            }
            return proveedor;
        }
        public bool Insert(Proveedor proveedor)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_proveedor", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Ruc", proveedor.Ruc);
                        command.Parameters.AddWithValue("@RazonSocial", proveedor.RazonSocial);
                        command.Parameters.AddWithValue("@Telefono", (object)proveedor.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Correo", (object)proveedor.Correo ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", (object)proveedor.Direccion ?? DBNull.Value);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            proveedor.IdProveedor = Convert.ToInt64(id);
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
        public bool Update(Proveedor proveedor)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_proveedor", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProveedor", proveedor.IdProveedor);
                        command.Parameters.AddWithValue("@Ruc", proveedor.Ruc);
                        command.Parameters.AddWithValue("@RazonSocial", proveedor.RazonSocial);
                        command.Parameters.AddWithValue("@Telefono", (object)proveedor.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Correo", (object)proveedor.Correo ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", (object)proveedor.Direccion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", proveedor.Estado);

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
        public bool Delete(long idProveedor)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_proveedor", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProveedor", idProveedor);

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