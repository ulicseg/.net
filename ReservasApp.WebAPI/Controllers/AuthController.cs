using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.WebAPI.DTOs;
using ReservasApp.WebAPI.Models;
using ReservasApp.WebAPI.Services;

namespace ReservasApp.WebAPI.Controllers
{
    /// <summary>
    /// Controlador de autenticación para la API
    /// ¿Por qué ApiController? Para comportamientos automáticos como validación de modelos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IJwtService jwtService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/auth/login
        /// Endpoint para iniciar sesión y obtener token JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Buscar usuario por email
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Credenciales inválidas",
                        Errors = new List<string> { "Email o contraseña incorrectos" }
                    });
                }

                // Verificar contraseña
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: true);
                
                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    
                    if (result.IsLockedOut)
                        errors.Add("Cuenta bloqueada temporalmente");
                    else
                        errors.Add("Email o contraseña incorrectos");

                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Error de autenticación",
                        Errors = errors
                    });
                }

                // Generar token JWT con roles
                var token = await _jwtService.GenerateTokenAsync(user);
                var userDto = _mapper.Map<UserDto>(user);

                // Obtener roles del usuario para incluir en la respuesta
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();

                _logger.LogInformation("Usuario {Email} autenticado exitosamente vía API con roles: {Roles}", 
                    user.Email, string.Join(", ", roles));

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    Expiration = _jwtService.GetTokenExpiration(),
                    User = userDto,
                    Message = "Autenticación exitosa"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login API");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Intente nuevamente más tarde" }
                });
            }
        }

        /// <summary>
        /// POST /api/auth/register
        /// Endpoint para registrar nuevo usuario
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                // Verificar si el usuario ya existe
                var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
                if (existingUser != null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "El usuario ya existe",
                        Errors = new List<string> { "Ya existe un usuario con este email" }
                    });
                }

                // Crear nuevo usuario
                var user = _mapper.Map<Usuario>(registerRequest);
                var result = await _userManager.CreateAsync(user, registerRequest.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Error al crear usuario",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Asignar rol "Cliente" por defecto
                await _userManager.AddToRoleAsync(user, "Cliente");

                // Generar token para login automático
                var token = await _jwtService.GenerateTokenAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                
                // Obtener roles del usuario recién creado
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();

                _logger.LogInformation("Usuario {Email} registrado exitosamente vía API con rol Cliente", user.Email);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    Expiration = _jwtService.GetTokenExpiration(),
                    User = userDto,
                    Message = "Usuario registrado exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro API");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Intente nuevamente más tarde" }
                });
            }
        }

        /// <summary>
        /// GET /api/auth/me
        /// Endpoint para obtener información del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token inválido"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    });
                }

                var userDto = _mapper.Map<UserDto>(user);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    User = userDto,
                    Message = "Usuario obtenido exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario actual");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// POST /api/auth/forgot-password
        /// Endpoint para solicitar recuperación de contraseña
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Por seguridad, no revelamos si el email existe o no
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Si el email existe, recibirás un enlace de recuperación"
                    });
                }

                // Generar token de recuperación
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Crear URL de callback (apuntará al frontend)
                var callbackUrl = $"https://localhost:5173/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(resetToken)}";

                // Enviar email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email!, resetToken, callbackUrl);

                if (!emailSent)
                {
                    _logger.LogWarning("No se pudo enviar email de recuperación a {Email}", user.Email);
                }

                _logger.LogInformation("Solicitud de recuperación de contraseña para {Email}", user.Email);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Si el email existe, recibirás un enlace de recuperación"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en recuperación de contraseña para {Email}", request.Email);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// POST /api/auth/reset-password
        /// Endpoint para restablecer contraseña con token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token inválido o expirado"
                    });
                }

                // Intentar restablecer la contraseña
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token inválido o expirado",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("Contraseña restablecida exitosamente para {Email}", user.Email);

                // Opcionalmente, enviar email de confirmación
                await _emailService.SendWelcomeEmailAsync(user.Email!, user.NombreCompleto);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Contraseña restablecida exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restableciendo contraseña para {Email}", request.Email);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}
