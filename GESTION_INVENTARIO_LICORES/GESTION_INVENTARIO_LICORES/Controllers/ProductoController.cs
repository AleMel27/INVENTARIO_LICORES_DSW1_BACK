using Microsoft.AspNetCore.Mvc;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductoController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.list();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Producto>> { Success = true, Message = "Productos obtenidos correctamente.", Data = lista });
            }
            return BadRequest(new Response<object> { Success = false, Message = "No se encontraron productos." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var producto = _service.getProducto(id);
            if (producto == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "El producto no existe." });
            }
            return Ok(new Response<Producto> { Success = true, Message = "Producto encontrado.", Data = producto });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Producto producto)
        {
            var resp = _service.insert(producto);
            if (resp)
            {
                return Created("", new Response<Producto> { Success = true, Message = "Producto registrado con éxito.", Data = producto });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al registrar el producto." });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Producto producto)
        {
            var existente = _service.getProducto(producto.IdProducto);
            if (existente == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "No se encontró el producto para actualizar." });
            }

            var resp = _service.update(producto);
            if (resp)
            {
                return Ok(new Response<Producto> { Success = true, Message = "Producto actualizado correctamente.", Data = producto });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al actualizar el producto." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existente = _service.getProducto(id);
            if (existente == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "No se encontró el producto para eliminar." });
            }

            var resp = _service.delete(id);
            if (resp)
            {
                return Ok(new Response<Producto> { Success = true, Message = "Producto eliminado con éxito.", Data = existente });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al eliminar el producto." });
        }
    }
}