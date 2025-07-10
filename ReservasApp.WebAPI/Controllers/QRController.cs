using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.WebAPI.DTOs;
using ReservasApp.WebAPI.Repositories;
using System.Security.Claims;

namespace ReservasApp.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para manejar acceso seguro via códigos QR
    /// ¿Por qué separado? Los QR tienen lógica específica de seguridad y expiración
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly IQRRepository _qrRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly ILogger<QRController> _logger;

        public QRController(
            IQRRepository qrRepository,
            IReservaRepository reservaRepository,
            ILogger<QRController> logger)
        {
            _qrRepository = qrRepository;
            _reservaRepository = reservaRepository;
            _logger = logger;
        }

        /// <summary>
        /// Acceso a información de reserva via QR
        /// ¿Por qué sin autenticación JWT? El QR debe ser accesible sin login
        /// </summary>
        [HttpGet("access/{hash}")]
        [AllowAnonymous]
        public async Task<ActionResult<QRAccessResponseDto>> AccessViaQR(string hash)
        {
            try
            {
                // Buscar el enlace QR por hash
                var qrLink = await _qrRepository.GetByHashAsync(hash);
                
                if (qrLink == null)
                {
                    _logger.LogWarning("Intento de acceso con hash QR inexistente: {Hash}", hash);
                    return NotFound(new { message = "Enlace QR no válido" });
                }

                // Verificar si ha expirado (10 minutos)
                if (qrLink.FechaExpiracion < DateTime.UtcNow)
                {
                    _logger.LogWarning("Intento de acceso con hash QR expirado: {Hash}, Expiró: {FechaExpiracion}", 
                        hash, qrLink.FechaExpiracion);
                    
                    // Opcional: eliminar enlaces expirados
                    if (qrLink.Id > 0)
                        await _qrRepository.DeleteAsync(qrLink.Id);
                    
                    return BadRequest(new { message = "El enlace QR ha expirado" });
                }

                // Obtener la reserva asociada
                if (!qrLink.ReservaId.HasValue)
                {
                    _logger.LogError("QR válido pero sin ReservaId asociada. QR ID: {QRId}", qrLink.Id);
                    return NotFound(new { message = "QR sin reserva asociada" });
                }

                var reserva = await _reservaRepository.GetByIdAsync(qrLink.ReservaId.Value);
                
                if (reserva == null)
                {
                    _logger.LogError("QR válido pero reserva no encontrada. QR ID: {QRId}, Reserva ID: {ReservaId}", 
                        qrLink.Id, qrLink.ReservaId.Value);
                    
                    return NotFound(new { message = "Reserva no encontrada" });
                }

                // Registrar acceso exitoso
                _logger.LogInformation("Acceso exitoso via QR: {Hash}, Reserva: {ReservaId}", hash, reserva.Id);

                // Responder con información limitada pero útil
                var response = new QRAccessResponseDto
                {
                    ReservaId = reserva.Id,
                    TipoServicio = reserva.TipoServicio.ToString(),
                    FechaReserva = reserva.FechaReserva,
                    Estado = reserva.Estado.ToString(),
                    Descripcion = reserva.Descripcion,
                    ClienteNombre = reserva.Usuario?.NombreCompleto ?? "Usuario",
                    FechaAcceso = DateTime.UtcNow,
                    MensajeAcceso = "Acceso autorizado via código QR"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar acceso QR con hash: {Hash}", hash);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Generar nuevo QR para una reserva (requiere autenticación)
        /// ¿Por qué protegido? Solo usuarios autenticados pueden generar QRs
        /// </summary>
        [HttpPost("generate/{reservaId}")]
        [Authorize]
        public async Task<ActionResult<QRGenerateResponseDto>> GenerateQR(int reservaId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no identificado" });
                }

                // Verificar que la reserva existe y pertenece al usuario
                var reserva = await _reservaRepository.GetByIdAsync(reservaId);
                if (reserva == null)
                {
                    return NotFound(new { message = "Reserva no encontrada" });
                }

                if (reserva.UsuarioId != userId)
                {
                    return Forbid("No tiene permisos para generar QR de esta reserva");
                }

                // Invalidar QRs anteriores para esta reserva
                await _qrRepository.InvalidateByReservaIdAsync(reservaId);

                // Crear nuevo QR
                var qrLink = await _qrRepository.CreateQRLinkAsync(reservaId);

                var response = new QRGenerateResponseDto
                {
                    Hash = qrLink.Hash,
                    QRUrl = $"{Request.Scheme}://{Request.Host}/api/qr/access/{qrLink.Hash}",
                    FechaExpiracion = qrLink.FechaExpiracion,
                    ReservaId = reservaId,
                    MinutosValidez = 10
                };

                _logger.LogInformation("QR generado exitosamente para reserva {ReservaId} por usuario {UserId}", 
                    reservaId, userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar QR para reserva: {ReservaId}", reservaId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Limpiar QRs expirados (endpoint de mantenimiento)
        /// ¿Por qué útil? Mantiene la base de datos limpia
        /// </summary>
        [HttpDelete("cleanup-expired")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<ActionResult> CleanupExpiredQRs()
        {
            try
            {
                var deletedCount = await _qrRepository.CleanupExpiredAsync();
                
                _logger.LogInformation("Limpieza de QRs expirados completada. Eliminados: {Count}", deletedCount);
                
                return Ok(new { 
                    message = "Limpieza completada", 
                    deletedCount = deletedCount 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante limpieza de QRs expirados");
                return StatusCode(500, new { message = "Error interno del servidor" });
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
