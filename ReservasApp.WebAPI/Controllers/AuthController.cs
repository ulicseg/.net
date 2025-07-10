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
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
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
    }
}
