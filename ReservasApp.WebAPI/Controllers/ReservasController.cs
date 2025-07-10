using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.WebAPI.DTOs;
using ReservasApp.WebAPI.Models;
using ReservasApp.WebAPI.Repositories;
using System.Security.Claims;

namespace ReservasApp.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para operaciones CRUD de reservas
    /// ¿Por qué Authorize? Para proteger todos los endpoints con JWT
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación JWT para todos los endpoints
    public class ReservasController : ControllerBase
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(
            IReservaRepository reservaRepository,
            IMapper mapper,
            ILogger<ReservasController> logger)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/reservas?page=1&limit=10
        /// Obtiene reservas paginadas del usuario autenticado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetReservas([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized();

                // Validar parámetros de paginación
                if (page < 1) page = 1;
                if (limit < 1 || limit > 100) limit = 10; // Limitar para evitar sobrecarga

                var reservas = await _reservaRepository.GetReservasPaginadasAsync(usuarioId, page, limit);
                var totalCount = await _reservaRepository.GetTotalReservasAsync(usuarioId);

                var reservasDto = _mapper.Map<List<ReservaListDto>>(reservas);

                var result = new PagedResultDto<ReservaListDto>
                {
                    Data = reservasDto,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = limit
                };

                _logger.LogInformation("Usuario {UserId} obtuvo {Count} reservas (página {Page})", 
                    usuarioId, reservasDto.Count, page);

                return Ok(ApiResponseDto<PagedResultDto<ReservaListDto>>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo reservas");
                return StatusCode(500, ApiResponseDto<PagedResultDto<ReservaListDto>>.ErrorResult(
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// GET /api/reservas/{id}
        /// Obtiene una reserva específica por ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetReserva(int id)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized();

                var reserva = await _reservaRepository.GetReservaPorIdAsync(id, usuarioId);
                if (reserva == null)
                {
                    return NotFound(ApiResponseDto<ReservaDetailDto>.ErrorResult(
                        "Reserva no encontrada"));
                }

                var reservaDto = _mapper.Map<ReservaDetailDto>(reserva);

                _logger.LogInformation("Usuario {UserId} obtuvo reserva {ReservaId}", usuarioId, id);

                return Ok(ApiResponseDto<ReservaDetailDto>.SuccessResult(reservaDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo reserva {ReservaId}", id);
                return StatusCode(500, ApiResponseDto<ReservaDetailDto>.ErrorResult(
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// POST /api/reservas
        /// Crea una nueva reserva
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] CreateReservaDto createReservaDto)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized();

                // Validaciones adicionales
                if (createReservaDto.FechaReserva <= DateTime.Now)
                {
                    return BadRequest(ApiResponseDto<ReservaDetailDto>.ErrorResult(
                        "La fecha de reserva debe ser futura"));
                }

                var reserva = _mapper.Map<Reserva>(createReservaDto);
                reserva.UsuarioId = usuarioId;

                var reservaCreada = await _reservaRepository.CrearReservaAsync(reserva);
                var reservaDto = _mapper.Map<ReservaDetailDto>(reservaCreada);

                _logger.LogInformation("Usuario {UserId} creó reserva {ReservaId}", usuarioId, reservaCreada.Id);

                return CreatedAtAction(
                    nameof(GetReserva),
                    new { id = reservaCreada.Id },
                    ApiResponseDto<ReservaDetailDto>.SuccessResult(reservaDto, "Reserva creada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando reserva");
                return StatusCode(500, ApiResponseDto<ReservaDetailDto>.ErrorResult(
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// PUT /api/reservas/{id}
        /// Actualiza una reserva existente
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarReserva(int id, [FromBody] UpdateReservaDto updateReservaDto)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized();

                // Verificar que la reserva existe y pertenece al usuario
                var existe = await _reservaRepository.ExisteReservaAsync(id, usuarioId);
                if (!existe)
                {
                    return NotFound(ApiResponseDto<ReservaDetailDto>.ErrorResult(
                        "Reserva no encontrada"));
                }

                // Validaciones adicionales
                if (updateReservaDto.FechaReserva <= DateTime.Now)
                {
                    return BadRequest(ApiResponseDto<ReservaDetailDto>.ErrorResult(
                        "La fecha de reserva debe ser futura"));
                }

                var reserva = _mapper.Map<Reserva>(updateReservaDto);
                reserva.Id = id;
                reserva.UsuarioId = usuarioId;

                var reservaActualizada = await _reservaRepository.ActualizarReservaAsync(reserva);
                var reservaDto = _mapper.Map<ReservaDetailDto>(reservaActualizada);

                _logger.LogInformation("Usuario {UserId} actualizó reserva {ReservaId}", usuarioId, id);

                return Ok(ApiResponseDto<ReservaDetailDto>.SuccessResult(reservaDto, "Reserva actualizada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando reserva {ReservaId}", id);
                return StatusCode(500, ApiResponseDto<ReservaDetailDto>.ErrorResult(
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// DELETE /api/reservas/{id}
        /// Elimina una reserva
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarReserva(int id)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized();

                var eliminada = await _reservaRepository.EliminarReservaAsync(id, usuarioId);
                if (!eliminada)
                {
                    return NotFound(ApiResponseDto<object>.ErrorResult(
                        "Reserva no encontrada"));
                }

                _logger.LogInformation("Usuario {UserId} eliminó reserva {ReservaId}", usuarioId, id);

                return Ok(ApiResponseDto<object>.SuccessResult(new { }, "Reserva eliminada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando reserva {ReservaId}", id);
                return StatusCode(500, ApiResponseDto<object>.ErrorResult(
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Helper method para obtener el ID del usuario autenticado
        /// ¿Por qué un helper? Para reutilizar código y mantener consistencia
        /// </summary>
        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
