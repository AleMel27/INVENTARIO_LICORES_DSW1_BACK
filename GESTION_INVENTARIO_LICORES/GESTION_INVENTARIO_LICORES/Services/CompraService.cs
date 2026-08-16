using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class CompraService : ICompraService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public CompraService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<CompraRespDto>> ListAsync(
            int pageNumber = 1,
            string? estado = null,
            long? idTipoComprobante = null,
            DateTime? fecha = null,
            string? razonSocial = null,
            string? numeroComprobante = null,
            string orden = "DESC"
        )
        {
            List<CompraRespDto> compras = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Compra_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoComprobante", (object?)idTipoComprobante ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Fecha", (object?)fecha ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RazonSocial", (object?)razonSocial ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroComprobante", (object?)numeroComprobante ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            compras.Add(new CompraRespDto
                            {
                                IdCompra = reader.GetInt64(0),

                                Proveedor = new ProveedorResumenRespDto
                                {
                                    IdProveedor = reader.GetInt64(1),
                                    Ruc = reader.GetString(2),
                                    RazonSocial = reader.GetString(3)
                                },

                                Usuario = new UsuarioResumenRespDto
                                {
                                    IdUsuario = reader.GetInt64(4),
                                    Nombres = reader.GetString(5),
                                    Apellidos = reader.GetString(6)
                                },

                                TipoComprobante = new TipoComprobanteRespDto
                                {
                                    IdTipoComprobante = reader.GetInt64(7),
                                    Nombre = reader.GetString(8)
                                },

                                FechaCompra = reader.GetDateTime(9),
                                NumeroComprobante = reader.GetString(10),
                                Total = reader.GetDecimal(11),
                                Estado = reader.GetString(12)
                            });
                        }
                    }
                }
            }
            int totalItems = compras.Count;

            List<CompraRespDto> items = compras
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<CompraRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<CompraDetalleRespDto?> GetDetailAsync(
            long idCompra
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Compra_ObtenerDetalle", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdCompra", idCompra);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            CompraDetalleRespDto compra = new CompraDetalleRespDto
                            {
                                IdCompra = reader.GetInt64(0),

                                Proveedor = new ProveedorResumenRespDto
                                {
                                    IdProveedor = reader.GetInt64(1),
                                    Ruc = reader.GetString(2),
                                    RazonSocial = reader.GetString(3)
                                },

                                Usuario = new UsuarioResumenRespDto
                                {
                                    IdUsuario = reader.GetInt64(4),
                                    Nombres = reader.GetString(5),
                                    Apellidos = reader.GetString(6)
                                },

                                TipoComprobante = new TipoComprobanteRespDto
                                {
                                    IdTipoComprobante = reader.GetInt64(7),
                                    Nombre = reader.GetString(8)
                                },

                                FechaCompra = reader.GetDateTime(9),
                                NumeroComprobante = reader.GetString(10),
                                Total = reader.GetDecimal(11),
                                Estado = reader.GetString(12),
                                Observacion = reader.IsDBNull(13) ? null : reader.GetString(13)
                            };

                            await reader.NextResultAsync();

                            while (await reader.ReadAsync())
                            {
                                compra.Detalles.Add(new DetalleCompraRespDto
                                {
                                    IdDetalleCompra = reader.GetInt64(0),

                                    Compra = new CompraResumenRespDto
                                    {
                                        IdCompra = reader.GetInt64(1),
                                        NumeroComprobante = reader.GetString(2)
                                    },

                                    Producto = new ProductoResumenRespDto
                                    {
                                        IdProducto = reader.GetInt64(3),
                                        Codigo = reader.GetString(4),
                                        Nombre = reader.GetString(5)
                                    },

                                    Cantidad = reader.GetInt32(6),
                                    CostoUnitario = reader.GetDecimal(7),
                                    Subtotal = reader.GetDecimal(8)
                                });
                            }

                            return compra;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<CompraDetalleRespDto?> CreateAsync(
            CompraReqDto request
        )
        {
            long? idCompra = null;

            DataTable detallesTable = new();

            detallesTable.Columns.Add(
                "IdProducto",
                typeof(long)
            );

            detallesTable.Columns.Add(
                "Cantidad",
                typeof(int)
            );

            detallesTable.Columns.Add(
                "CostoUnitario",
                typeof(decimal)
            );

            foreach (DetalleCompraReqDto detalle in request.Detalles)
            {
                detallesTable.Rows.Add(
                    detalle.IdProducto,
                    detalle.Cantidad,
                    detalle.CostoUnitario
                );
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Compra_Crear", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProveedor", request.IdProveedor);
                    command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                    command.Parameters.AddWithValue("@IdTipoComprobante", request.IdTipoComprobante);
                    command.Parameters.AddWithValue("@NumeroComprobante", request.NumeroComprobante);
                    command.Parameters.AddWithValue("@Observacion", (object?)request.Observacion ?? DBNull.Value);

                    SqlParameter detallesParameter = command.Parameters.AddWithValue("@Detalles", detallesTable);
                    detallesParameter.SqlDbType = SqlDbType.Structured;
                    detallesParameter.TypeName = "dbo.DetalleCompraTvp";

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            idCompra = reader.GetInt64(0);
                        }
                    }
                }
            }

            if (!idCompra.HasValue)
            {
                return null;
            }

            return await GetDetailAsync(idCompra.Value);
        }

        public async Task<bool> ChangeStatusAsync(
            long idCompra,
            EstadoCompraReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Compra_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCompra", idCompra);
                    command.Parameters.AddWithValue("@NuevoEstado", request.Estado);

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
