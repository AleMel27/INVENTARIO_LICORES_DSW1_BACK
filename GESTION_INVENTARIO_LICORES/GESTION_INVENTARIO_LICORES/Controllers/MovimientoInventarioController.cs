using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN,ALMACENERO")]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _service;

        public MovimientoInventarioController(
            IMovimientoInventarioService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            string? codigoProducto = null,
            string? nombreProducto = null,
            long? idAlmacen = null,
            string? numeroComprobante = null,
            long? idTipoMovimiento = null,
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

            if (idTipoMovimiento.HasValue && idTipoMovimiento.Value <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdTipoMovimiento debe ser mayor a 0."
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

            try
            {
                PaginatedRespDto<MovimientoInventarioRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        codigoProducto,
                        nombreProducto,
                        idAlmacen,
                        numeroComprobante,
                        idTipoMovimiento,
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
                        Detail = "Ocurrió un error al obtener los movimientos de inventario."
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

        [HttpGet("{idMovimiento}")]
        public async Task<IActionResult> GetById(
            long idMovimiento
        )
        {
            if (idMovimiento <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdMovimiento debe ser mayor a 0."
                    }
                );
            }

            try
            {
                MovimientoInventarioRespDto? movimiento =
                    await _service.GetByIdAsync(idMovimiento);

                if (movimiento is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Movimiento no encontrado",
                            Detail = "El movimiento de inventario solicitado no existe."
                        }
                    );
                }

                return Ok(movimiento);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener el movimiento de inventario."
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
