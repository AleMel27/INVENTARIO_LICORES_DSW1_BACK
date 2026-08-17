using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _service;
        private readonly ILogger<CompraController> _logger;

        public CompraController(
            ICompraService service,
            ILogger<CompraController> logger
        )
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> Get(
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
            if (pageNumber <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El número de página debe ser mayor a 0."
                    }
                );
            }

            if (idAlmacen.HasValue && idAlmacen.Value <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdAlmacen debe ser mayor a 0."
                    }
                );
            }

            orden = orden.ToUpperInvariant();

            if (orden != "ASC" && orden != "DESC")
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El orden solamente puede ser ASC o DESC."
                    }
                );
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                estado = estado.ToUpperInvariant();

                if (estado != "PENDIENTE" &&
                    estado != "RECIBIDA" &&
                    estado != "CANCELADA")
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "El estado solamente puede ser PENDIENTE, RECIBIDA o CANCELADA."
                        }
                    );
                }
            }

            try
            {
                PaginatedRespDto<CompraRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        estado,
                        idTipoComprobante,
                        idAlmacen,
                        fecha,
                        razonSocial,
                        numeroComprobante,
                        orden
                    );

                return Ok(resultado);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener las compras."
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno del servidor."
                    }
                );
            }
        }

        [HttpGet("{idCompra}")]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> GetById(
            long idCompra
        )
        {
            if (idCompra <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCompra debe ser mayor a 0."
                    }
                );
            }

            try
            {
                CompraDetalleRespDto? compra =
                    await _service.GetDetailAsync(idCompra);

                if (compra is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Compra no encontrada",
                            Detail = "La compra solicitada no existe."
                        }
                    );
                }

                return Ok(compra);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener la compra."
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno del servidor."
                    }
                );
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> Post(
            [FromBody] CompraReqDto request
        )
        {
            try
            {
                CompraDetalleRespDto? compra =
                    await _service.CreateAsync(request);

                if (compra is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar la compra."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idCompra = compra.IdCompra
                    },
                    compra
                );
            }
            catch (SqlException ex)
                when (ex.Number == 2601 ||
                      ex.Number == 2627)
            {
                return Conflict(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflicto",
                        Detail = "Ya existe una compra registrada con ese comprobante para el proveedor."
                    }
                );
            }
            catch (ConflictException ex)
            {
                return Conflict(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflicto",
                        Detail = ex.Message
                    }
                );
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = ex.Message
                    }
                );
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al registrar la compra."
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno del servidor."
                    }
                );
            }
        }

        [HttpPatch("{idCompra}/estado")]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> ChangeStatus(
            long idCompra,
            [FromBody] EstadoCompraReqDto request
        )
        {
            if (idCompra <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCompra debe ser mayor a 0."
                    }
                );
            }

            try
            {
                CompraDetalleRespDto? compra =
                    await _service.GetDetailAsync(idCompra);

                if (compra is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Compra no encontrada",
                            Detail = "No se encontró la compra solicitada."
                        }
                    );
                }

                string estadoActual =
                    compra.Estado.ToUpperInvariant();

                string nuevoEstado =
                    request.Estado.ToUpperInvariant();

                if (nuevoEstado != "RECIBIDA" &&
                    nuevoEstado != "CANCELADA")
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "El nuevo estado solamente puede ser RECIBIDA o CANCELADA."
                        }
                    );
                }

                if (estadoActual == nuevoEstado)
                {
                    return Conflict(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Conflicto de estado",
                            Detail = "La compra ya se encuentra en el estado solicitado."
                        }
                    );
                }

                if (estadoActual != "PENDIENTE")
                {
                    return Conflict(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Conflicto de estado",
                            Detail = "La compra ya se encuentra en un estado final y no puede modificarse."
                        }
                    );
                }

                if (nuevoEstado == "CANCELADA" &&
                    !User.IsInRole("ADMIN"))
                {
                    return Forbid();
                }

                long idUsuarioMovimiento = 0;

                if (nuevoEstado == "RECIBIDA")
                {
                    string? idUsuarioClaim =
                        User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!long.TryParse(idUsuarioClaim, out idUsuarioMovimiento) ||
                        idUsuarioMovimiento <= 0)
                    {
                        return BadRequest(
                            new ProblemDetails
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Title = "Solicitud inválida",
                                Detail = "No se pudo identificar al usuario que recibe la compra."
                            }
                        );
                    }
                }

                request.Estado = nuevoEstado;

                bool cambiado =
                    await _service.ChangeStatusAsync(
                        idCompra,
                        request,
                        idUsuarioMovimiento
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado de la compra."
                        }
                    );
                }

                CompraDetalleRespDto? actualizada =
                    await _service.GetDetailAsync(idCompra);

                return Ok(actualizada);
            }
            catch (ConflictException ex)
            {
                return Conflict(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflicto de estado",
                        Detail = ex.Message
                    }
                );
            }
            catch (BusinessValidationException ex)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = ex.Message
                    }
                );
            }
            catch (SqlException ex)
                when (ex.Number == 50030)
            {
                _logger.LogError(
                    ex,
                    "Error SQL al cambiar el estado de la compra {IdCompra}. Number: {Number}",
                    idCompra,
                    ex.Number
                );

                return NotFound(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Compra no encontrada",
                        Detail = "No se encontró la compra solicitada."
                    }
                );
            }
            catch (SqlException ex)
                when (ex.Number == 50031 ||
                      ex.Number == 50037 ||
                      ex.Number == 50038)
            {
                _logger.LogError(
                    ex,
                    "Error SQL al cambiar el estado de la compra {IdCompra}. Number: {Number}",
                    idCompra,
                    ex.Number
                );

                return Conflict(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflicto de estado",
                        Detail = ex.Number == 50037
                            ? "No todos los productos de la compra tienen inventario configurado en el almacén seleccionado."
                            : "No se pudo completar el cambio de estado de la compra."
                    }
                );
            }
            catch (SqlException ex)
                when (ex.Number == 50032 ||
                      ex.Number == 50033 ||
                      ex.Number == 50034 ||
                      ex.Number == 50035 ||
                      ex.Number == 50036)
            {
                _logger.LogError(
                    ex,
                    "Error SQL al cambiar el estado de la compra {IdCompra}. Number: {Number}",
                    idCompra,
                    ex.Number
                );

                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "La compra no cumple las condiciones necesarias para ser recibida."
                    }
                );
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "Error SQL inesperado al cambiar el estado de la compra {IdCompra}. Number: {Number}",
                    idCompra,
                    ex.Number
                );

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al cambiar el estado de la compra."
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno del servidor."
                    }
                );
            }
        }
    }
}
