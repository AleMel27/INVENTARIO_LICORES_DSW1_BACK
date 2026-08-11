using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class DetalleCompraService : IDetalleCompraService
    {
        private readonly string? _conexion;

        public DetalleCompraService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<DetalleCompra> ListAll()
        {
            List<DetalleCompra> lista = new List<DetalleCompra>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_detalle_compra", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new DetalleCompra
                            {
                                IdDetalleCompra = reader.GetInt64(reader.GetOrdinal("IdDetalleCompra")),
                                IdCompra = reader.GetInt64(reader.GetOrdinal("IdCompra")),
                                IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                CostoUnitario = reader.GetDecimal(reader.GetOrdinal("CostoUnitario")),
                                Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),

                                Producto = new Producto
                                {
                                    IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                    Codigo = reader.GetString(reader.GetOrdinal("CodigoProducto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Producto"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<DetalleCompra> ListByCompra(long idCompra)
        {
            List<DetalleCompra> lista = new List<DetalleCompra>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_detalle_compra_by_id", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCompra", idCompra);

                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new DetalleCompra
                            {
                                IdDetalleCompra = reader.GetInt64(reader.GetOrdinal("IdDetalleCompra")),
                                IdCompra = reader.GetInt64(reader.GetOrdinal("IdCompra")),
                                IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                CostoUnitario = reader.GetDecimal(reader.GetOrdinal("CostoUnitario")),
                                Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),

                                Producto = new Producto
                                {
                                    IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                    Codigo = reader.GetString(reader.GetOrdinal("CodigoProducto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Producto"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insert(DetalleCompra detalle)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_detalle_compra", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdCompra", detalle.IdCompra);
                        command.Parameters.AddWithValue("@IdProducto", detalle.IdProducto);
                        command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        command.Parameters.AddWithValue("@CostoUnitario", detalle.CostoUnitario);

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