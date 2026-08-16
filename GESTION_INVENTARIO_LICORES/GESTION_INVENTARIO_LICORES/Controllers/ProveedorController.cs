using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Enums;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _service;

        public ProveedorController(
            IProveedorService service
        )
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
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

                PaginatedRespDto<ProveedorRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
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
                        Detail = "Ocurrió un error interno al procesar el proveedor."
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

        [HttpGet("{idProveedor}")]
        [Authorize(Roles = "ADMIN,ALMACENERO")]
        public async Task<IActionResult> GetById(
            long idProveedor
        )
        {
            if (idProveedor <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProveedor debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProveedorRespDto? proveedor =
                    await _service.GetByIdAsync(idProveedor);

                if (proveedor is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Proveedor no encontrado",
                            Detail = "El proveedor solicitado no existe."
                        }
                    );
                }

                return Ok(proveedor);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno al procesar el proveedor."
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
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post(
            [FromBody] ProveedorReqDto request
        )
        {
            try
            {
                ProveedorRespDto? proveedor =
                    await _service.CreateAsync(request);

                if (proveedor is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar el proveedor."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idProveedor = proveedor.IdProveedor
                    },
                    proveedor
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
                        Detail = "Ya existe un proveedor con el RUC o correo indicado."
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
                        Detail = "Ocurrió un error interno al procesar el proveedor."
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

        [HttpPut("{idProveedor}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(
            long idProveedor,
            [FromBody] ProveedorUpdateReqDto request
        )
        {
            if (idProveedor <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProveedor debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProveedorRespDto? existente =
                    await _service.GetByIdAsync(idProveedor);

                if (existente is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Proveedor no encontrado",
                            Detail = "No se encontró el proveedor para actualizar."
                        }
                    );
                }

                ProveedorRespDto? actualizado =
                    await _service.UpdateAsync(
                        idProveedor,
                        request
                    );

                if (actualizado is null)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Error interno",
                            Detail = "No se pudo obtener el proveedor actualizado."
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
                        Detail = "Ya existe un proveedor con el RUC o correo indicado."
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
                        Detail = "Ocurrió un error interno al procesar el proveedor."
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

        [HttpPatch("{idProveedor}/estado")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeStatus(
            long idProveedor,
            [FromQuery] bool estado
        )
        {
            if (idProveedor <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProveedor debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProveedorRespDto? proveedor =
                    await _service.GetByIdAsync(idProveedor);

                if (proveedor is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Proveedor no encontrado",
                            Detail = "No se encontró el proveedor solicitado."
                        }
                    );
                }

                if (proveedor.Estado == estado)
                {
                    string mensajeConflicto = estado
                        ? "El proveedor ya se encuentra activo."
                        : "El proveedor ya se encuentra inactivo.";

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
                        idProveedor,
                        estado
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado del proveedor."
                        }
                    );
                }

                ProveedorRespDto? actualizado =
                    await _service.GetByIdAsync(idProveedor);

                if (actualizado is null)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Error interno",
                            Detail = "No se pudo obtener el proveedor actualizado."
                        }
                    );
                }

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
                        Detail = "Ocurrió un error interno al procesar el proveedor."
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
