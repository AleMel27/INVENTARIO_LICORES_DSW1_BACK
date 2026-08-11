using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;
using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly string? _conexion;

        public InventarioService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Inventario> List()
        {
            List<Inventario> lista = new List<Inventario>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_inventario", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inventario
                            {
                                IdInventario = reader.GetInt64(reader.GetOrdinal("IdInventario")),
                                IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                IdAlmacen = reader.GetInt64(reader.GetOrdinal("IdAlmacen")),
                                StockActual = reader.GetInt32(reader.GetOrdinal("StockActual")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion")),

                                Producto = new Producto
                                {
                                    IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto")),
                                    Codigo = reader.GetString(reader.GetOrdinal("CodigoProducto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Producto"))
                                },
                                Almacen = new Almacen
                                {
                                    IdAlmacen = reader.GetInt64(reader.GetOrdinal("IdAlmacen")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Almacen"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool AjustarInventario(InventarioDto ajuste)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_ajustar_inventario_manual", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdProducto", ajuste.IdProducto);
                        command.Parameters.AddWithValue("@IdAlmacen", ajuste.IdAlmacen);
                        command.Parameters.AddWithValue("@IdUsuario", ajuste.IdUsuario);
                        command.Parameters.AddWithValue("@Cantidad", ajuste.Cantidad);
                        command.Parameters.AddWithValue("@TipoAjuste", ajuste.TipoAjuste);
                        command.Parameters.AddWithValue("@Motivo", ajuste.Motivo);

                        command.ExecuteNonQuery();
                        resp = true;

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