using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(
            IAuthService service
        )
        {
            _service = service;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginReqDto request
        )
        {
            try
            {
                LoginRespDto? resultado =
                    await _service.LoginAsync(request);

                if (resultado is null)
                {
                    return Unauthorized(
                        new ProblemDetails
                        {
                            Status = StatusCodes.Status401Unauthorized,
                            Title = "Autenticación fallida",
                            Detail = "Las credenciales proporcionadas no son válidas."
                        }
                    );
                }

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
                        Detail = "Ocurrió un error interno durante la autenticación."
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
                        Detail = "Ocurrió un error interno durante la autenticación."
                    }
                );
            }
        }
    }
}
