using Microsoft.AspNetCore.Mvc;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _service;

        public ProveedorController(IProveedorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.list();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Proveedor>> { Success = true, Message = "Proveedores obtenidos correctamente.", Data = lista });
            }
            return BadRequest(new Response<object> { Success = false, Message = "No se encontraron proveedores." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var proveedor = _service.getProveedor(id);
            if (proveedor == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "El proveedor no existe." });
            }
            return Ok(new Response<Proveedor> { Success = true, Message = "Proveedor encontrado.", Data = proveedor });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Proveedor proveedor)
        {
            var resp = _service.insert(proveedor);
            if (resp)
            {
                return Created("", new Response<Proveedor> { Success = true, Message = "Proveedor registrado con éxito.", Data = proveedor });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al registrar el proveedor." });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Proveedor proveedor)
        {
            var existente = _service.getProveedor(proveedor.IdProveedor);
            if (existente == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "No se encontró el proveedor para actualizar." });
            }

            var resp = _service.update(proveedor);
            if (resp)
            {
                return Ok(new Response<Proveedor> { Success = true, Message = "Proveedor actualizado correctamente.", Data = proveedor });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al actualizar el proveedor." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existente = _service.getProveedor(id);
            if (existente == null)
            {
                return NotFound(new Response<object> { Success = false, Message = "No se encontró el proveedor para eliminar." });
            }

            var resp = _service.delete(id);
            if (resp)
            {
                return Ok(new Response<Proveedor> { Success = true, Message = "Proveedor eliminado con éxito.", Data = existente });
            }
            return BadRequest(new Response<object> { Success = false, Message = "Error al eliminar el proveedor." });
        }
    }
}