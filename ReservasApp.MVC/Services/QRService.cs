using Microsoft.EntityFrameworkCore;
using QRCoder;
using ReservasApp.MVC.Data;
using ReservasApp.MVC.Models;

namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Servicio para generar y gestionar códigos QR
    /// ¿Por qué este servicio? Para centralizar toda la lógica de QR y seguridad temporal
    /// </summary>
    public class QRService : IQRService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public QRService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Crea un enlace QR para una reserva específica
        /// ¿Por qué generar un enlace en BD? Para tener control total sobre expiración y uso único
        /// </summary>
        public async Task<QRLink> CrearQRParaReservaAsync(int reservaId, string accion = "VerDetalle")
        {
            // Verificar que la reserva existe
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
                throw new ArgumentException("La reserva no existe", nameof(reservaId));

            // Crear el enlace QR
            var qrLink = QRLink.CrearParaReserva(reservaId, accion);
            
            _context.QRLinks.Add(qrLink);
            await _context.SaveChangesAsync();

            return qrLink;
        }

        /// <summary>
        /// Valida si un hash QR es válido y no ha expirado
        /// ¿Por qué validación completa? Seguridad: verificar expiración, uso único, etc.
        /// </summary>
        public async Task<QRLink?> ValidarQRAsync(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return null;

            var qrLink = await _context.QRLinks
                .Include(q => q.Reserva)
                .ThenInclude(r => r!.Usuario)
                .FirstOrDefaultAsync(q => q.Hash == hash);

            // Verificar si existe y está vigente
            if (qrLink == null || !qrLink.EstaVigente)
                return null;

            return qrLink;
        }

        /// <summary>
        /// Genera la imagen QR en formato Base64
        /// ¿Por qué Base64? Para poder mostrar la imagen directamente en HTML sin archivos temporales
        /// </summary>
        public string GenerarImagenQR(string url)
        {
            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);
                
                return Convert.ToBase64String(qrCodeBytes);
            }
            catch (Exception ex)
            {
                // Log del error (en producción usarías ILogger)
                Console.WriteLine($"Error generando QR: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Marca un QR como usado para evitar reutilización
        /// ¿Por qué marcar como usado? Seguridad: algunos QR pueden ser de uso único
        /// </summary>
        public async Task<bool> MarcarQRComoUsadoAsync(string hash)
        {
            var qrLink = await _context.QRLinks
                .FirstOrDefaultAsync(q => q.Hash == hash);

            if (qrLink == null)
                return false;

            qrLink.Usado = true;
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Tarea de limpieza para eliminar QRs expirados
        /// ¿Por qué limpiar? Para mantener la BD optimizada y no acumular datos innecesarios
        /// </summary>
        public async Task LimpiarQRsExpiradosAsync()
        {
            var fechaLimite = DateTime.UtcNow.AddDays(-1); // Eliminar QRs expirados desde hace 1 día
            
            var qrsExpirados = await _context.QRLinks
                .Where(q => q.FechaExpiracion < fechaLimite)
                .ToListAsync();

            if (qrsExpirados.Any())
            {
                _context.QRLinks.RemoveRange(qrsExpirados);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Obtiene la URL base para los enlaces QR
        /// </summary>
        public string ObtenerUrlBase()
        {
            return _configuration["BaseUrl"] ?? "https://localhost:5001";
        }

        /// <summary>
        /// Construye la URL completa para un hash QR
        /// </summary>
        public string ConstruirUrlQR(string hash)
        {
            var baseUrl = ObtenerUrlBase();
            return $"{baseUrl}/QR/{hash}";
        }
    }
}
