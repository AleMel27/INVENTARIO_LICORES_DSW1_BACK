using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
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
            long? idAlmacen = null,
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
                    command.Parameters.AddWithValue("@IdAlmacen", (object?)idAlmacen ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Fecha", (object?)fecha ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RazonSocial", (object?)razonSocial ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroComprobante", (object?)numeroComprobante ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int ordIdAlmacen =
                            reader.GetOrdinal("IdAlmacen");

                        int ordNombreAlmacen =
                            reader.GetOrdinal("NombreAlmacen");

                        int ordUbicacionAlmacen =
                            reader.GetOrdinal("UbicacionAlmacen");

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

                                Almacen = new AlmacenInventarioRespDto
                                {
                                    IdAlmacen = reader.GetInt64(ordIdAlmacen),
                                    Nombre = reader.GetString(ordNombreAlmacen),
                                    Ubicacion = reader.GetString(ordUbicacionAlmacen)
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
                        int ordIdAlmacen =
                            reader.GetOrdinal("IdAlmacen");

                        int ordNombreAlmacen =
                            reader.GetOrdinal("NombreAlmacen");

                        int ordUbicacionAlmacen =
                            reader.GetOrdinal("UbicacionAlmacen");

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

                                Almacen = new AlmacenInventarioRespDto
                                {
                                    IdAlmacen = reader.GetInt64(ordIdAlmacen),
                                    Nombre = reader.GetString(ordNombreAlmacen),
                                    Ubicacion = reader.GetString(ordUbicacionAlmacen)
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

            if (request.Detalles.Count == 0)
            {
                throw new BusinessValidationException(
                    "Debe agregar al menos un producto a la compra."
                );
            }

            bool tieneProductosDuplicados =
                request.Detalles
                    .GroupBy(detalle => detalle.IdProducto)
                    .Any(grupo => grupo.Count() > 1);

            if (tieneProductosDuplicados)
            {
                throw new ConflictException(
                    "No se puede registrar una compra con productos duplicados en el detalle."
                );
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (!await ExisteProveedorActivoAsync(con, request.IdProveedor))
                {
                    throw new BusinessValidationException(
                        "El proveedor indicado no es válido o se encuentra inactivo."
                    );
                }

                if (!await ExisteUsuarioActivoAsync(con, request.IdUsuario))
                {
                    throw new BusinessValidationException(
                        "El usuario indicado no es válido o se encuentra inactivo."
                    );
                }

                if (!await ExisteTipoComprobanteActivoAsync(
                    con,
                    request.IdTipoComprobante
                ))
                {
                    throw new BusinessValidationException(
                        "El tipo de comprobante indicado no es válido o se encuentra inactivo."
                    );
                }

                if (!await ExisteAlmacenActivoAsync(con, request.IdAlmacen))
                {
                    throw new BusinessValidationException(
                        "El almacén indicado no es válido o se encuentra inactivo."
                    );
                }

                if (await ExisteComprobanteAsync(
                    con,
                    request.IdProveedor,
                    request.IdTipoComprobante,
                    request.NumeroComprobante
                ))
                {
                    throw new ConflictException(
                        "Ya existe una compra registrada con ese comprobante."
                    );
                }

                foreach (DetalleCompraReqDto detalle in request.Detalles)
                {
                    if (!await ExisteProductoActivoAsync(con, detalle.IdProducto))
                    {
                        throw new BusinessValidationException(
                            "Uno o más productos del detalle no son válidos o se encuentran inactivos."
                        );
                    }
                }

                DataTable detallesTable = CrearDetallesTable(request.Detalles);

                using (SqlCommand command =
                    new SqlCommand(
                        "sp_Compra_Crear",
                        con
                    ))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProveedor", request.IdProveedor);
                    command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                    command.Parameters.AddWithValue("@IdTipoComprobante", request.IdTipoComprobante);
                    command.Parameters.AddWithValue("@IdAlmacen", request.IdAlmacen);
                    command.Parameters.AddWithValue("@NumeroComprobante", request.NumeroComprobante);
                    command.Parameters.AddWithValue("@Observacion", (object?)request.Observacion ?? DBNull.Value);

                    SqlParameter detallesParameter = command.Parameters.AddWithValue("@Detalles", detallesTable);
                    detallesParameter.SqlDbType = SqlDbType.Structured;
                    detallesParameter.TypeName = "dbo.DetalleCompraTvp";

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

        private static DataTable CrearDetallesTable(
            List<DetalleCompraReqDto> detalles
        )
        {
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

            foreach (DetalleCompraReqDto detalle in detalles)
            {
                detallesTable.Rows.Add(
                    detalle.IdProducto,
                    detalle.Cantidad,
                    detalle.CostoUnitario
                );
            }

            return detallesTable;
        }

        private async Task<bool> ExisteProveedorActivoAsync(
            SqlConnection con,
            long idProveedor
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Proveedor_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProveedor", idProveedor);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteUsuarioActivoAsync(
            SqlConnection con,
            long idUsuario
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Usuario_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteTipoComprobanteActivoAsync(
            SqlConnection con,
            long idTipoComprobante
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_TipoComprobante_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(
                    "@IdTipoComprobante",
                    idTipoComprobante
                );

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteAlmacenActivoAsync(
            SqlConnection con,
            long idAlmacen
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Almacen_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteComprobanteAsync(
            SqlConnection con,
            long idProveedor,
            long idTipoComprobante,
            string numeroComprobante
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Compra_ExisteComprobante", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                command.Parameters.AddWithValue(
                    "@IdTipoComprobante",
                    idTipoComprobante
                );
                command.Parameters.AddWithValue(
                    "@NumeroComprobante",
                    numeroComprobante
                );

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteProductoActivoAsync(
            SqlConnection con,
            long idProducto
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Producto_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProducto", idProducto);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<string?> ObtenerEstadoCompraAsync(
            SqlConnection con,
            long idCompra
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Compra_ObtenerDetalle", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCompra", idCompra);

                using (SqlDataReader reader =
                    await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader.GetString(reader.GetOrdinal("Estado"));
                    }
                }
            }

            return null;
        }

        public async Task<bool> ChangeStatusAsync(
            long idCompra,
            EstadoCompraReqDto request,
            long idUsuarioMovimiento
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                string? estadoActual =
                    await ObtenerEstadoCompraAsync(
                        con,
                        idCompra
                    );

                if (estadoActual is null)
                {
                    return false;
                }

                string nuevoEstado =
                    request.Estado.ToUpperInvariant();

                estadoActual = estadoActual.ToUpperInvariant();

                if (nuevoEstado != "RECIBIDA" &&
                    nuevoEstado != "CANCELADA")
                {
                    throw new BusinessValidationException(
                        "El nuevo estado solamente puede ser RECIBIDA o CANCELADA."
                    );
                }

                if (estadoActual == nuevoEstado)
                {
                    throw new ConflictException(
                        "La compra ya se encuentra en el estado solicitado."
                    );
                }

                if (estadoActual != "PENDIENTE")
                {
                    throw new ConflictException(
                        "La compra ya se encuentra en un estado final y no puede modificarse."
                    );
                }

                if (nuevoEstado == "RECIBIDA" &&
                    idUsuarioMovimiento <= 0)
                {
                    throw new BusinessValidationException(
                        "No se pudo identificar al usuario que recibe la compra."
                    );
                }

                string storedProcedure = nuevoEstado == "RECIBIDA"
                    ? "sp_Compra_Recibir"
                    : "sp_Compra_CambiarEstado";

                using (SqlCommand command =
                    new SqlCommand(
                        storedProcedure,
                        con
                    ))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCompra", idCompra);

                    if (nuevoEstado == "CANCELADA")
                    {
                        command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                    }
                    else
                    {
                        command.Parameters.AddWithValue(
                            "@IdUsuarioMovimiento",
                            idUsuarioMovimiento
                        );
                    }

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
