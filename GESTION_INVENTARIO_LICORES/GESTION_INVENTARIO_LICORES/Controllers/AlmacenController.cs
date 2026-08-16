using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Enums;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenController : ControllerBase
    {
        private readonly IAlmacenService _service;

        public AlmacenController(
            IAlmacenService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            string? nombre = null,
            string? ubicacion = null,
            EstadoFiltro estado = EstadoFiltro.Activos,
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
                bool? estadoFiltro = estado switch
                {
                    EstadoFiltro.Activos => true,
                    EstadoFiltro.Inactivos => false,
                    EstadoFiltro.Todos => null,
                    _ => true
                };

                PaginatedRespDto<AlmacenRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        nombre,
                        ubicacion,
                        estadoFiltro,
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
                        Detail = "Ocurrió un error al obtener los almacenes."
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

        [HttpGet("{idAlmacen}")]
        public async Task<IActionResult> GetById(
            long idAlmacen
        )
        {
            if (idAlmacen <= 0)
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

            try
            {
                AlmacenRespDto? almacen =
                    await _service.GetByIdAsync(idAlmacen);

                if (almacen is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Almacén no encontrado",
                            Detail = "El almacén solicitado no existe."
                        }
                    );
                }

                return Ok(almacen);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener el almacén."
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
        public async Task<IActionResult> Post(
            [FromBody] AlmacenReqDto request
        )
        {
            try
            {
                AlmacenRespDto? almacen =
                    await _service.CreateAsync(request);

                if (almacen is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar el almacén."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idAlmacen = almacen.IdAlmacen
                    },
                    almacen
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
                        Detail = "Ya existe un almacén con ese nombre."
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
                        Detail = "Ocurrió un error al registrar el almacén."
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

        [HttpPut("{idAlmacen}")]
        public async Task<IActionResult> Put(
            long idAlmacen,
            [FromBody] AlmacenUpdateReqDto request
        )
        {
            if (idAlmacen <= 0)
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

            try
            {
                AlmacenRespDto? existente =
                    await _service.GetByIdAsync(idAlmacen);

                if (existente is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Almacén no encontrado",
                            Detail = "No se encontró el almacén para actualizar."
                        }
                    );
                }

                AlmacenRespDto? actualizado =
                    await _service.UpdateAsync(
                        idAlmacen,
                        request
                    );

                if (actualizado is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Almacén no encontrado",
                            Detail = "No se encontró el almacén para actualizar."
                        }
                    );
                }

                return Ok(actualizado);
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
                        Detail = "Ya existe un almacén con ese nombre."
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
                        Detail = "Ocurrió un error al actualizar el almacén."
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

        [HttpPatch("{idAlmacen}/estado")]
        public async Task<IActionResult> ChangeStatus(
            long idAlmacen,
            [FromQuery] bool estado
        )
        {
            if (idAlmacen <= 0)
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

            try
            {
                AlmacenRespDto? almacen =
                    await _service.GetByIdAsync(idAlmacen);

                if (almacen is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Almacén no encontrado",
                            Detail = "No se encontró el almacén solicitado."
                        }
                    );
                }

                if (almacen.Estado == estado)
                {
                    string mensajeConflicto = estado
                        ? "El almacén ya se encuentra activo."
                        : "El almacén ya se encuentra inactivo.";

                    return Conflict(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Conflicto",
                            Detail = mensajeConflicto
                        }
                    );
                }

                bool cambiado =
                    await _service.ChangeStatusAsync(
                        idAlmacen,
                        estado
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado del almacén."
                        }
                    );
                }

                AlmacenRespDto? actualizado =
                    await _service.GetByIdAsync(idAlmacen);

                return Ok(actualizado);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al cambiar el estado del almacén."
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
