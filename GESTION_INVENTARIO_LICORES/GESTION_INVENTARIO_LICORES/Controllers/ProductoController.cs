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
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductoController(
            IProductoService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            long? idMarca = null,
            long? idCategoria = null,
            string? codigo = null,
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

            if (idMarca.HasValue && idMarca.Value <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdMarca debe ser mayor a 0."
                    }
                );
            }

            if (idCategoria.HasValue && idCategoria.Value <= 0)
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

                PaginatedRespDto<ProductoRespDto> resultado =
                    await _service.ListAsync(
                        pageNumber,
                        idMarca,
                        idCategoria,
                        codigo,
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
                        Detail = "Ocurrió un error interno al procesar el producto."
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

        [HttpGet("{idProducto}")]
        public async Task<IActionResult> GetById(
            long idProducto
        )
        {
            if (idProducto <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProducto debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProductoRespDto? producto =
                    await _service.GetByIdAsync(idProducto);

                if (producto is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Producto no encontrado",
                            Detail = "El producto solicitado no existe."
                        }
                    );
                }

                return Ok(producto);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno al procesar el producto."
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
            [FromBody] ProductoReqDto request
        )
        {
            try
            {
                ProductoRespDto? producto =
                    await _service.CreateAsync(request);

                if (producto is null)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo registrar el producto."
                        }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idProducto = producto.IdProducto
                    },
                    producto
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
                        Detail = "Ya existe un producto registrado con ese código."
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
                        Detail = "La categoría o marca indicada no es válida."
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
                        Detail = "Ocurrió un error interno al procesar el producto."
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

        [HttpPut("{idProducto}")]
        public async Task<IActionResult> Put(
            long idProducto,
            [FromBody] ProductoUpdateReqDto request
        )
        {
            if (idProducto <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProducto debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProductoRespDto? existente =
                    await _service.GetByIdAsync(idProducto);

                if (existente is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Producto no encontrado",
                            Detail = "No se encontró el producto para actualizar."
                        }
                    );
                }

                ProductoRespDto? actualizado =
                    await _service.UpdateAsync(
                        idProducto,
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
                            Detail = "No se pudo obtener el producto actualizado."
                        }
                    );
                }

                return Ok(actualizado);
            }
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "La categoría o marca indicada no es válida."
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
                        Detail = "Ocurrió un error interno al procesar el producto."
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

        [HttpPatch("{idProducto}/estado")]
        public async Task<IActionResult> ChangeStatus(
            long idProducto,
            [FromQuery] bool estado
        )
        {
            if (idProducto <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdProducto debe ser mayor a 0."
                    }
                );
            }

            try
            {
                ProductoRespDto? producto =
                    await _service.GetByIdAsync(idProducto);

                if (producto is null)
                {
                    return NotFound(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Producto no encontrado",
                            Detail = "No se encontró el producto solicitado."
                        }
                    );
                }

                if (producto.Estado == estado)
                {
                    string mensajeConflicto = estado
                        ? "El producto ya se encuentra activo."
                        : "El producto ya se encuentra inactivo.";

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
                        idProducto,
                        estado
                    );

                if (!cambiado)
                {
                    return BadRequest(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Solicitud inválida",
                            Detail = "No se pudo cambiar el estado del producto."
                        }
                    );
                }

                ProductoRespDto? actualizado =
                    await _service.GetByIdAsync(idProducto);

                if (actualizado is null)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Error interno",
                            Detail = "No se pudo obtener el producto actualizado."
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
                        Detail = "Ocurrió un error interno al procesar el producto."
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
