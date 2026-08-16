using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class JwtService : IJwtService
    {
        private readonly string jwtKey;
        private readonly string jwtIssuer;
        private readonly string jwtAudience;
        private readonly int expiresMinutes;

        public JwtService(IConfiguration configuration)
        {
            jwtKey =
                configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Key."
                );

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "La clave JWT no está configurada. Configure Jwt:Key mediante "
                    + "User Secrets o una variable de entorno."
                );
            }

            jwtIssuer =
                configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Issuer."
                );

            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                throw new InvalidOperationException(
                    "La configuración Jwt:Issuer no puede estar vacía."
                );
            }

            jwtAudience =
                configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "No se encontró la configuración Jwt:Audience."
                );

            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                throw new InvalidOperationException(
                    "La configuración Jwt:Audience no puede estar vacía."
                );
            }

            expiresMinutes =
                configuration.GetValue<int>(
                    "Jwt:ExpiresMinutes"
                );

            if (expiresMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "La configuración Jwt:ExpiresMinutes debe ser mayor que 0."
                );
            }
        }

        public string GenerateToken(
            long idUsuario,
            string correo,
            string rol
        )
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(idUsuario),
                    "El id del usuario debe ser mayor que 0."
                );
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentException(
                    "El correo del usuario es obligatorio.",
                    nameof(correo)
                );
            }

            if (string.IsNullOrWhiteSpace(rol))
            {
                throw new ArgumentException(
                    "El rol del usuario es obligatorio.",
                    nameof(rol)
                );
            }

            DateTime now = DateTime.UtcNow;

            Claim[] claims =
            {
                new Claim(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, correo),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            SigningCredentials credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    notBefore: now,
                    expires: now.AddMinutes(expiresMinutes),
                    signingCredentials: credentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
