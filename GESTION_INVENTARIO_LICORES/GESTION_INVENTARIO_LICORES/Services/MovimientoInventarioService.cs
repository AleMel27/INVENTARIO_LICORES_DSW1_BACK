using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        private readonly string? _conexion;

        public MovimientoInventarioService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<KardexReporteDto> ConsultarKardex(long? idAlmacen = null, long? idProducto = null, string? tipoMovimiento = null)
        {
            List<KardexReporteDto> lista = new List<KardexReporteDto>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_consultar_kardex", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdAlmacen", (object?)idAlmacen ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdProducto", (object?)idProducto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoMovimiento", (object?)tipoMovimiento ?? DBNull.Value);

                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new KardexReporteDto
                            {
                                IdMovimiento = reader.GetInt64(reader.GetOrdinal("IdMovimiento")),
                                FechaMovimiento = reader.GetDateTime(reader.GetOrdinal("FechaMovimiento")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                Producto = reader.GetString(reader.GetOrdinal("Producto")),
                                Almacen = reader.GetString(reader.GetOrdinal("Almacen")),
                                TipoMovimiento = reader.GetString(reader.GetOrdinal("TipoMovimiento")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                StockAnterior = reader.GetInt32(reader.GetOrdinal("StockAnterior")),
                                StockPosterior = reader.GetInt32(reader.GetOrdinal("StockPosterior")),
                                Motivo = reader.GetString(reader.GetOrdinal("Motivo")),
                                Referencia = reader.IsDBNull(reader.GetOrdinal("Referencia")) ? null : reader.GetString(reader.GetOrdinal("Referencia")),
                                UsuarioResponsable = reader.GetString(reader.GetOrdinal("UsuarioResponsable"))
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}