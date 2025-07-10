using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.Services
{
    /// <summary>
    /// Servicio para generar y validar tokens JWT
    /// ¿Por qué JWT? Para autenticación stateless que escala bien en APIs
    /// </summary>
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(Usuario usuario);
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiration();
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration, UserManager<Usuario> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _secret = _configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JWT Secret not configured");
            _issuer = _configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JWT Issuer not configured");
            _audience = _configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JWT Audience not configured");
            _expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
        }

        /// <summary>
        /// Genera un token JWT para un usuario autenticado incluyendo sus roles
        /// ¿Por qué async? Para obtener roles del usuario desde la base de datos
        /// </summary>
        public async Task<string> GenerateTokenAsync(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            // Claims del usuario
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id),
                new("sub", usuario.Id), // Subject claim estándar
                new(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new(ClaimTypes.Name, usuario.NombreCompleto),
                new("nombre", usuario.Nombre),
                new("apellido", usuario.Apellido),
                new("fechaRegistro", usuario.FechaRegistro.ToString("yyyy-MM-dd")),
                new("jti", Guid.NewGuid().ToString()) // JWT ID único
            };

            // Agregar roles del usuario
            var roles = await _userManager.GetRolesAsync(usuario);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Si no tiene roles, asignar Cliente por defecto
            if (!roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "Cliente"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Valida un token JWTSecretKey
        /// ¿Por qué validar? Para asegurar que el token no ha sido manipulado
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene la fecha de expiración para incluir en las respuestas
        /// </summary>
        public DateTime GetTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_expirationMinutes);
        }
    }
}
