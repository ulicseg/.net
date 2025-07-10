using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.WebAPI.DTOs;
using ReservasApp.WebAPI.Repositories;
using System.Security.Claims;

namespace ReservasApp.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para manejar acceso seguro por códigos QR
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
        /// Acceso a información de reserva via QR (JSON)
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
        /// Vista visual HTML para acceso QR - Nueva funcionalidad mejorada
        /// ¿Por qué separado? Para mostrar interfaz amigable en lugar de JSON
        /// </summary>
        [HttpGet("view/{hash}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewViaQR(string hash)
        {
            try
            {
                // Reutilizar la misma lógica de validación
                var qrLink = await _qrRepository.GetByHashAsync(hash);
                
                if (qrLink == null)
                {
                    return Content(GenerateErrorHtml("Enlace QR no válido", "El código QR que intentas usar no es válido o ha sido eliminado."), "text/html");
                }

                if (qrLink.FechaExpiracion < DateTime.UtcNow)
                {
                    if (qrLink.Id > 0)
                        await _qrRepository.DeleteAsync(qrLink.Id);
                    
                    return Content(GenerateErrorHtml("Enlace QR expirado", "Este código QR ha expirado. Los códigos QR son válidos por 10 minutos."), "text/html");
                }

                if (!qrLink.ReservaId.HasValue)
                {
                    return Content(GenerateErrorHtml("Error en QR", "Este código QR no tiene una reserva asociada válida."), "text/html");
                }

                var reserva = await _reservaRepository.GetByIdAsync(qrLink.ReservaId.Value);
                
                if (reserva == null)
                {
                    return Content(GenerateErrorHtml("Reserva no encontrada", "La reserva asociada a este código QR no fue encontrada."), "text/html");
                }

                // Generar HTML visual
                var html = GenerateReservaHtml(reserva, hash);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar vista QR con hash: {Hash}", hash);
                return Content(GenerateErrorHtml("Error del sistema", "Ocurrió un error al procesar tu solicitud."), "text/html");
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
                    QRUrl = $"{Request.Scheme}://{Request.Host}/api/qr/view/{qrLink.Hash}",
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

        /// <summary>
        /// Genera HTML de error para QR inválidos
        /// </summary>
        private string GenerateErrorHtml(string titulo, string mensaje)
        {
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Error - Código QR</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        .container {{
            background: white;
            padding: 2rem;
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            text-align: center;
            max-width: 500px;
            width: 100%;
        }}
        .error-icon {{
            font-size: 4rem;
            color: #ff6b6b;
            margin-bottom: 1rem;
        }}
        h1 {{
            color: #2c3e50;
            margin-bottom: 1rem;
            font-size: 1.8rem;
        }}
        p {{
            color: #7f8c8d;
            line-height: 1.6;
            margin-bottom: 2rem;
        }}
        .btn {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 25px;
            text-decoration: none;
            display: inline-block;
            font-weight: 500;
            transition: transform 0.2s;
        }}
        .btn:hover {{
            transform: translateY(-2px);
        }}
        @media print {{
            body {{ background: white; }}
            .btn {{ display: none; }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='error-icon'>⚠️</div>
        <h1>{titulo}</h1>
        <p>{mensaje}</p>
        <a href='#' onclick='window.close()' class='btn'>Cerrar</a>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Genera HTML visual para mostrar información de reserva
        /// </summary>
        private string GenerateReservaHtml(Models.Reserva reserva, string hash)
        {
            var fechaFormateada = reserva.FechaReserva.ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm", new System.Globalization.CultureInfo("es-ES"));
            var tipoFormateado = reserva.TipoServicio.ToString().Replace("Consulta", "Consulta ").Replace("Terapia", "Terapia ").Replace("Examen", "Examen de ").Replace("Cirugia", "Cirugía ");
            
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Información de Reserva - QR</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        .container {{
            background: white;
            padding: 2.5rem;
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            max-width: 600px;
            width: 100%;
        }}
        .header {{
            text-align: center;
            margin-bottom: 2rem;
        }}
        .qr-icon {{
            font-size: 3rem;
            margin-bottom: 1rem;
        }}
        .title {{
            color: #2c3e50;
            margin: 0;
            font-size: 2rem;
            font-weight: 700;
        }}
        .subtitle {{
            color: #3498db;
            margin: 0.5rem 0 0 0;
            font-size: 1.1rem;
            font-weight: 500;
        }}
        .info-card {{
            background: #f8f9fa;
            padding: 1.5rem;
            border-radius: 15px;
            margin: 1.5rem 0;
            border-left: 5px solid #3498db;
        }}
        .info-row {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0.8rem 0;
            border-bottom: 1px solid #ecf0f1;
        }}
        .info-row:last-child {{
            border-bottom: none;
        }}
        .info-label {{
            font-weight: 600;
            color: #2c3e50;
            display: flex;
            align-items: center;
        }}
        .info-label::before {{
            content: attr(data-icon);
            margin-right: 0.5rem;
            font-size: 1.2rem;
        }}
        .info-value {{
            color: #34495e;
            font-weight: 500;
            text-align: right;
            flex: 1;
            margin-left: 1rem;
        }}
        .estado {{
            background: #27ae60;
            color: white;
            padding: 0.3rem 1rem;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 600;
        }}
        .descripcion {{
            background: #ecf0f1;
            padding: 1rem;
            border-radius: 10px;
            margin-top: 1rem;
            font-style: italic;
            color: #7f8c8d;
            line-height: 1.5;
        }}
        .footer {{
            text-align: center;
            margin-top: 2rem;
            padding-top: 1.5rem;
            border-top: 2px solid #ecf0f1;
        }}
        .access-time {{
            color: #7f8c8d;
            font-size: 0.9rem;
            margin-bottom: 1rem;
        }}
        .btn {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 25px;
            text-decoration: none;
            display: inline-block;
            font-weight: 500;
            margin: 0 10px;
            transition: transform 0.2s;
            cursor: pointer;
        }}
        .btn:hover {{
            transform: translateY(-2px);
        }}
        .btn-secondary {{
            background: linear-gradient(135deg, #95a5a6 0%, #7f8c8d 100%);
        }}
        @media print {{
            body {{
                background: white;
                padding: 0;
            }}
            .btn {{
                display: none;
            }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='qr-icon'>📋</div>
            <h1 class='title'>Información de Reserva</h1>
            <p class='subtitle'>✅ Acceso autorizado via código QR</p>
        </div>

        <div class='info-card'>
            <div class='info-row'>
                <span class='info-label' data-icon='🆔'>ID de Reserva:</span>
                <span class='info-value'>#{reserva.Id:D6}</span>
            </div>
            <div class='info-row'>
                <span class='info-label' data-icon='👤'>Cliente:</span>
                <span class='info-value'>{reserva.Usuario?.NombreCompleto ?? "Usuario"}</span>
            </div>
            <div class='info-row'>
                <span class='info-label' data-icon='🏥'>Tipo de Servicio:</span>
                <span class='info-value'>{tipoFormateado}</span>
            </div>
            <div class='info-row'>
                <span class='info-label' data-icon='📅'>Fecha y Hora:</span>
                <span class='info-value'>{fechaFormateada}</span>
            </div>
            <div class='info-row'>
                <span class='info-label' data-icon='📊'>Estado:</span>
                <span class='info-value'><span class='estado'>{reserva.Estado}</span></span>
            </div>
            
            {(string.IsNullOrEmpty(reserva.Descripcion) ? "" : $@"
            <div class='descripcion'>
                <strong>📝 Descripción:</strong><br>
                {reserva.Descripcion}
            </div>")}
        </div>

        <div class='footer'>
            <p class='access-time'>🕐 Accedido el {DateTime.Now:dd/MM/yyyy 'a las' HH:mm}</p>
            <button onclick='window.print()' class='btn'>🖨️ Imprimir</button>
            <button onclick='window.close()' class='btn btn-secondary'>✖️ Cerrar</button>
        </div>
    </div>

    <script>
        // Auto-close en 30 minutos por seguridad
        setTimeout(() => {{
            if (confirm('Esta ventana se cerrará automáticamente por seguridad. ¿Deseas mantenerla abierta?')) {{
                // Reiniciar timer
                setTimeout(arguments.callee, 1800000); // 30 min más
            }} else {{
                window.close();
            }}
        }}, 1800000); // 30 minutos
    </script>
</body>
</html>";
        }
    }
}
