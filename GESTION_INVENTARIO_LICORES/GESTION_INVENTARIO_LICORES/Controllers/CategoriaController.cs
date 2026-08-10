using Microsoft.AspNetCore.Mvc;
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
            var lista = _service.list();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Categoria>>
                {
                    Success = true,
                    Message = "Categorías obtenidas correctamente.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron categorías."
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var cat = _service.getCategoria(id);
            if (cat == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "La categoría no existe."
                });
            }
            return Ok(new Response<Categoria>
            {
                Success = true,
                Message = "Categoría encontrada.",
                Data = cat
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post(Categoria categoria)
        {
            var resp = _service.insert(categoria);
            if (resp)
            {
                return Created("", new Response<Categoria>
                {
                    Success = true,
                    Message = "Categoría registrada con éxito.",
                    Data = categoria
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al registrar la categoría."
            });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Categoria categoria)
        {
            var existente = _service.getCategoria(categoria.IdCategoria);
            if (existente == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la categoría para actualizar."
                });
            }

            var resp = _service.update(categoria);
            if (resp)
            {
                return Ok(new Response<Categoria>
                {
                    Success = true,
                    Message = "Categoría actualizada correctamente.",
                    Data = categoria
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al actualizar la categoría."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existente = _service.getCategoria(id);
            if (existente == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la categoría para eliminar."
                });
            }

            var resp = _service.delete(id);
            if (resp)
            {
                return Ok(new Response<Categoria>
                {
                    Success = true,
                    Message = "Categoría eliminada con éxito.",
                    Data = existente
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al eliminar la categoría."
            });
        }
    }
}