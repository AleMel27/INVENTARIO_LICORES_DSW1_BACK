using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class ProductoService : IProductoService
    {

        private readonly string? conexion;

        public ProductoService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion");
        }

        public List<Producto> list()
        {
            List<Producto> temporal = new List<Producto>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_list_productos", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Producto producto = new Producto
                            {
                                IdProducto = reader.GetInt64(0),
                                IdCategoria = reader.GetInt64(1),
                                IdMarca = reader.GetInt64(2),
                                Codigo = reader.GetString(3),
                                Nombre = reader.GetString(4),
                                Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CapacidadMl = reader.GetInt32(6),
                                GradoAlcoholico = reader.GetDecimal(7),
                                PrecioVenta = reader.GetDecimal(8),
                                StockMinimo = reader.GetInt32(9),
                                Estado = reader.GetBoolean(10),
                                FechaCreacion = reader.GetDateTime(11),
                                FechaActualizacion = reader.GetDateTime(12)
                            };
                            temporal.Add(producto);
                        }
                    }
                }
            }
            return temporal;
        }

        public Producto getProducto(long idProducto)
        {
            Producto? producto = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_find_producto_by_id", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", idProducto);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                IdProducto = reader.GetInt64(0),
                                IdCategoria = reader.GetInt64(1),
                                IdMarca = reader.GetInt64(2),
                                Codigo = reader.GetString(3),
                                Nombre = reader.GetString(4),
                                Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CapacidadMl = reader.GetInt32(6),
                                GradoAlcoholico = reader.GetDecimal(7),
                                PrecioVenta = reader.GetDecimal(8),
                                StockMinimo = reader.GetInt32(9),
                                Estado = reader.GetBoolean(10),
                                FechaCreacion = reader.GetDateTime(11),
                                FechaActualizacion = reader.GetDateTime(12)
                            };
                        }
                    }
                }
            }
            return producto;
        }

        public bool insert(Producto producto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_producto", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                        command.Parameters.AddWithValue("@IdMarca", producto.IdMarca);
                        command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", (object?)producto.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CapacidadMl", producto.CapacidadMl);
                        command.Parameters.AddWithValue("@GradoAlcoholico", producto.GradoAlcoholico);
                        command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                        command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);

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

        public bool update(Producto producto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_producto", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                        command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                        command.Parameters.AddWithValue("@IdMarca", producto.IdMarca);
                        command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", (object?)producto.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CapacidadMl", producto.CapacidadMl);
                        command.Parameters.AddWithValue("@GradoAlcoholico", producto.GradoAlcoholico);
                        command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                        command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                        command.Parameters.AddWithValue("@Estado", producto.Estado);

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

        public bool delete(long idProducto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_producto", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProducto", idProducto);

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
    }
}
