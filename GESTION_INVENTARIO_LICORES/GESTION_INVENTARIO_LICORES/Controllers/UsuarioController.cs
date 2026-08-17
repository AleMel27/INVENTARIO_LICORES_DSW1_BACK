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
    //[Authorize(Roles = "ADMIN")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(
            IUsuarioService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            string? nombres = null,
            string? apellidos = null,
            long? idRol = null,
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

            if (idRol.HasValue && idRol.Value <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdRol debe ser mayor a 0."
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

                PaginatedRespDto<UsuarioRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        nombres,
                        apellidos,
                        idRol,
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
                        Detail = "Ocurrió un error al obtener los usuarios."
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

        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> GetById(
            long idUsuario
        )
        {
            if (idUsuario <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdUsuario debe ser mayor a 0."
                    }
                );
            }

            try
            {
                UsuarioRespDto? usuario =
                    await _service.GetByIdAsync(idUsuario);

                if (usuario is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Usuario no encontrado",
                            Detail = "El usuario solicitado no existe."
                        }
                    );
                }

                return Ok(usuario);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener el usuario."
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
            [FromBody] UsuarioReqDto request
        )
        {
            try
            {
                UsuarioRespDto? usuario =
                    await _service.CreateAsync(request);

                if (usuario is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar el usuario."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idUsuario = usuario.IdUsuario
                    },
                    usuario
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
                        Detail = "Ya existe un usuario registrado con ese correo."
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
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El rol indicado no es válido."
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
                        Detail = "Ocurrió un error al registrar el usuario."
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

        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> Put(
            long idUsuario,
            [FromBody] UsuarioUpdateReqDto request
        )
        {
            if (idUsuario <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdUsuario debe ser mayor a 0."
                    }
                );
            }

            try
            {
                UsuarioRespDto? existente =
                    await _service.GetByIdAsync(idUsuario);

                if (existente is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Usuario no encontrado",
                            Detail = "No se encontró el usuario para actualizar."
                        }
                    );
                }

                UsuarioRespDto? actualizado =
                    await _service.UpdateAsync(
                        idUsuario,
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
                            Detail = "No se pudo obtener el usuario actualizado."
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
                        Detail = "Ya existe un usuario registrado con ese correo."
                    }
                );
            }
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El rol indicado no es válido."
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
                        Detail = "Ocurrió un error al actualizar el usuario."
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

        [HttpPatch("{idUsuario}/estado")]
        public async Task<IActionResult> ChangeStatus(
            long idUsuario,
            [FromQuery] bool estado
        )
        {
            if (idUsuario <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdUsuario debe ser mayor a 0."
                    }
                );
            }

            try
            {
                UsuarioRespDto? usuario =
                    await _service.GetByIdAsync(idUsuario);

                if (usuario is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Usuario no encontrado",
                            Detail = "No se encontró el usuario solicitado."
                        }
                    );
                }

                if (usuario.Estado == estado)
                {
                    string mensajeConflicto = estado
                        ? "El usuario ya se encuentra activo."
                        : "El usuario ya se encuentra inactivo.";

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
                        idUsuario,
                        estado
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado del usuario."
                        }
                    );
                }

                UsuarioRespDto? actualizado =
                    await _service.GetByIdAsync(idUsuario);

                if (actualizado is null)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Error interno",
                            Detail = "No se pudo obtener el usuario actualizado."
                        }
                    );
                }

                return Ok(actualizado);
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
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al cambiar el estado del usuario."
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
