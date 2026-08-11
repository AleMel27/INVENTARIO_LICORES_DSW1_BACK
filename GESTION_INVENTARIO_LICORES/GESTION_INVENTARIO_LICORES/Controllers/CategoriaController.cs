using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Categoria>>
                {
                    Success = true,
                    Message = "Categorías obtenidas con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron categorías registradas."
            });
        }

        [HttpGet("{idCategoria}")]
        public async Task<IActionResult> GetById(long idCategoria)
        {
            var categoria = _service.GetCategoria(idCategoria);
            if (categoria == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "La categoría solicitada no existe."
                });
            }
            return Ok(new Response<Categoria>
            {
                Success = true,
                Message = "Categoría encontrada.",
                Data = categoria
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                var resp = _service.Insert(categoria);
                if (resp)
                {
                    return Created("", new Response<Categoria>
                    {
                        Success = true,
                        Message = "Categoría registrada correctamente.",
                        Data = categoria
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar la categoría."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Categoria categoria)
        {
            var existe = _service.GetCategoria(categoria.IdCategoria);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la categoría para actualizar."
                });
            }

            try
            {
                var resp = _service.Update(categoria);
                if (resp)
                {
                    return Ok(new Response<Categoria>
                    {
                        Success = true,
                        Message = "Se actualizó la categoría correctamente.",
                        Data = categoria
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar la categoría."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{idCategoria}")]
        public async Task<IActionResult> Delete(long idCategoria)
        {
            var categoria = _service.GetCategoria(idCategoria);
            if (categoria == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la categoría para dar de baja."
                });
            }

            try
            {
                var resp = _service.Delete(idCategoria);
                if (resp)
                {
                    categoria.Estado = false;
                    return Ok(new Response<Categoria>
                    {
                        Success = true,
                        Message = "Se dio de baja la categoría correctamente.",
                        Data = categoria
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja la categoría."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}