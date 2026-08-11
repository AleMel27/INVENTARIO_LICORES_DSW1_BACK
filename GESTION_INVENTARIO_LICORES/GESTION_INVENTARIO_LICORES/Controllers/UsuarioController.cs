using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Usuario>>
                {
                    Success = true,
                    Message = "Usuarios obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron usuarios registrados."
            });
        }

        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> GetById(long idUsuario)
        {
            var usuario = _service.GetUsuario(idUsuario);
            if (usuario == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "El usuario solicitado no existe."
                });
            }
            return Ok(new Response<Usuario>
            {
                Success = true,
                Message = "Usuario encontrado.",
                Data = usuario
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                var resp = _service.Insert(usuario);
                if (resp)
                {
                    return Created("", new Response<Usuario>
                    {
                        Success = true,
                        Message = "Usuario registrado correctamente.",
                        Data = usuario
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar el usuario."
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
        public async Task<IActionResult> Put([FromBody] Usuario usuario)
        {
            var existe = _service.GetUsuario(usuario.IdUsuario);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el usuario para actualizar."
                });
            }

            try
            {
                var resp = _service.Update(usuario);
                if (resp)
                {
                    return Ok(new Response<Usuario>
                    {
                        Success = true,
                        Message = "Se actualizó el usuario correctamente.",
                        Data = usuario
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar el usuario."
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

        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> Delete(long idUsuario)
        {
            var usuario = _service.GetUsuario(idUsuario);
            if (usuario == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el usuario para dar de baja."
                });
            }

            try
            {
                var resp = _service.Delete(idUsuario);
                if (resp)
                {
                    usuario.Estado = false;
                    return Ok(new Response<Usuario>
                    {
                        Success = true,
                        Message = "Se dio de baja al usuario correctamente.",
                        Data = usuario
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja al usuario."
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

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var existe = _service.GetUsuario(request.IdUsuario);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el usuario para cambiar la contraseña."
                });
            }

            try
            {
                var resp = _service.ChangePassword(request.IdUsuario, request.NuevoPasswordHash);
                if (resp)
                {
                    return Ok(new Response<object>
                    {
                        Success = true,
                        Message = "Contraseña actualizada correctamente."
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar la contraseña."
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

    // DTO auxiliar para recibir la petición de cambio de clave de forma segura
    public class ChangePasswordRequest
    {
        public long IdUsuario { get; set; }
        public string NuevoPasswordHash { get; set; } = string.Empty;
    }
}