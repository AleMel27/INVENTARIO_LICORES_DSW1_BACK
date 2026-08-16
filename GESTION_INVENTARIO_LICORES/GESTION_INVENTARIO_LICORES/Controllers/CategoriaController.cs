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
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriaController(
            ICategoriaService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            string? nombre = null,
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

                PaginatedRespDto<CategoriaRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        nombre,
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
                        Detail = "Ocurrió un error al obtener las categorías."
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

        [HttpGet("{idCategoria}")]
        public async Task<IActionResult> GetById(
            long idCategoria
        )
        {
            if (idCategoria <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCategoria debe ser mayor a 0."
                    }
                );
            }

            try
            {
                CategoriaRespDto? categoria =
                    await _service.GetByIdAsync(idCategoria);

                if (categoria is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Categoría no encontrada",
                            Detail = "La categoría solicitada no existe."
                        }
                    );
                }

                return Ok(categoria);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener la categoría."
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
            [FromBody] CategoriaReqDto request
        )
        {
            try
            {
                CategoriaRespDto? categoria =
                    await _service.CreateAsync(request);

                if (categoria is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar la categoría."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idCategoria = categoria.IdCategoria
                    },
                    categoria
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
                        Detail = "Ya existe una categoría con ese nombre."
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
                        Detail = "Ocurrió un error al registrar la categoría."
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

        [HttpPut("{idCategoria}")]
        public async Task<IActionResult> Put(
            long idCategoria,
            [FromBody] CategoriaUpdateReqDto request
        )
        {
            if (idCategoria <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCategoria debe ser mayor a 0."
                    }
                );
            }

            try
            {
                CategoriaRespDto? existente =
                    await _service.GetByIdAsync(idCategoria);

                if (existente is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Categoría no encontrada",
                            Detail = "No se encontró la categoría para actualizar."
                        }
                    );
                }

                CategoriaRespDto? actualizada =
                    await _service.UpdateAsync(
                        idCategoria,
                        request
                    );

                if (actualizada is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Categoría no encontrada",
                            Detail = "No se encontró la categoría para actualizar."
                        }
                    );
                }

                return Ok(actualizada);
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
                        Detail = "Ya existe una categoría con ese nombre."
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
                        Detail = "Ocurrió un error al actualizar la categoría."
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

        [HttpPatch("{idCategoria}/estado")]
        public async Task<IActionResult> ChangeStatus(
            long idCategoria,
            [FromQuery] bool estado
        )
        {
            if (idCategoria <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCategoria debe ser mayor a 0."
                    }
                );
            }

            try
            {
                CategoriaRespDto? categoria =
                    await _service.GetByIdAsync(idCategoria);

                if (categoria is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Categoría no encontrada",
                            Detail = "No se encontró la categoría solicitada."
                        }
                    );
                }

                if (categoria.Estado == estado)
                {
                    string mensajeConflicto = estado
                        ? "La categoría ya se encuentra activa."
                        : "La categoría ya se encuentra inactiva.";

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
                        idCategoria,
                        estado
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado de la categoría."
                        }
                    );
                }

                CategoriaRespDto? actualizada =
                    await _service.GetByIdAsync(idCategoria);

                return Ok(actualizada);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al cambiar el estado de la categoría."
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
