using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class ProductoService : IProductoService
    {
        private readonly string? _conexion;

        public ProductoService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Producto> List()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_productos", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Producto
                            {
                                IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("Producto")), // Mapea 'Producto' según tu alias SQL
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                CapacidadMl = reader.GetInt32(reader.GetOrdinal("CapacidadMl")),
                                GradoAlcoholico = reader.GetDecimal(reader.GetOrdinal("GradoAlcoholico")),
                                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                                IdCategoria = reader.GetInt64(reader.GetOrdinal("IdCategoria")),
                                IdMarca = reader.GetInt64(reader.GetOrdinal("IdMarca")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion")),

                                // Inicializamos las propiedades de navegación que expone el JOIN
                                Categoria = new Categoria
                                {
                                    IdCategoria = reader.GetInt64(reader.GetOrdinal("IdCategoria")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Categoria"))
                                },
                                Marca = new Marca
                                {
                                    IdMarca = reader.GetInt64(reader.GetOrdinal("IdMarca")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Marca"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Producto GetProducto(long idProducto)
        {
            Producto producto = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_producto", con))
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
                                IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                IdCategoria = reader.GetInt64(reader.GetOrdinal("IdCategoria")),
                                IdMarca = reader.GetInt64(reader.GetOrdinal("IdMarca")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")), // Aquí se llama 'Nombre' en tu SP de búsqueda
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                CapacidadMl = reader.GetInt32(reader.GetOrdinal("CapacidadMl")),
                                GradoAlcoholico = reader.GetDecimal(reader.GetOrdinal("GradoAlcoholico")),
                                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                                StockMinimo = reader.GetInt32(reader.GetOrdinal("StockMinimo")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion"))
                            };
                        }
                    }
                }
            }
            return producto;
        }

        public bool Insert(Producto producto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
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
                        command.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CapacidadMl", producto.CapacidadMl);
                        command.Parameters.AddWithValue("@GradoAlcoholico", producto.GradoAlcoholico);
                        command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                        command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            producto.IdProducto = Convert.ToInt64(id);
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

        public bool Update(Producto producto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
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
                        command.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CapacidadMl", producto.CapacidadMl);
                        command.Parameters.AddWithValue("@GradoAlcoholico", producto.GradoAlcoholico);
                        command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                        command.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                        command.Parameters.AddWithValue("@Estado", producto.Estado);

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

        public bool Delete(long idProducto)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
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