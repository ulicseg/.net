using Microsoft.EntityFrameworkCore;
using ReservasApp.WebAPI.Data;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de QR Links
    /// ¿Por qué repositorio separado? Porque los QR tienen lógica específica diferente a las reservas
    /// </summary>
    public interface IQRRepository
    {
        Task<QRLink> CrearQRAsync(QRLink qrLink);
        Task<QRLink?> GetQRPorHashAsync(string hash);
        Task<bool> MarcarComoUsadoAsync(string hash);
        Task LimpiarQRsExpiradosAsync();
        Task<IEnumerable<QRLink>> GetQRsVigentesAsync();
        
        // Nuevos métodos para el controlador
        Task<QRLink?> GetByHashAsync(string hash);
        Task<QRLink> CreateQRLinkAsync(int reservaId);
        Task InvalidateByReservaIdAsync(int reservaId);
        Task<int> CleanupExpiredAsync();
        Task DeleteAsync(int id);
    }

    /// <summary>
    /// Implementación del repositorio de QR Links
    /// </summary>
    public class QRRepository : IQRRepository
    {
        private readonly ApplicationDbContext _context;

        public QRRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo enlace QR
        /// </summary>
        public async Task<QRLink> CrearQRAsync(QRLink qrLink)
        {
            _context.QRLinks.Add(qrLink);
            await _context.SaveChangesAsync();
            return qrLink;
        }

        /// <summary>
        /// Obtiene un QR por su hash incluyendo la reserva relacionada
        /// ¿Por qué Include? Para obtener datos de la reserva en una sola consulta
        /// </summary>
        public async Task<QRLink?> GetQRPorHashAsync(string hash)
        {
            return await _context.QRLinks
                .Include(q => q.Reserva)
                .ThenInclude(r => r!.Usuario)
                .FirstOrDefaultAsync(q => q.Hash == hash);
        }

        /// <summary>
        /// Marca un QR como usado para evitar reutilización
        /// ¿Por qué marcar como usado? Seguridad: algunos QR pueden ser de uso único
        /// </summary>
        public async Task<bool> MarcarComoUsadoAsync(string hash)
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
        /// Limpia QRs expirados para mantener la base de datos optimizada
        /// ¿Por qué limpiar? Para evitar acumulación de datos innecesarios
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
        /// Obtiene todos los QRs vigentes (útil para administración)
        /// </summary>
        public async Task<IEnumerable<QRLink>> GetQRsVigentesAsync()
        {
            return await _context.QRLinks
                .Where(q => !q.Usado && q.FechaExpiracion > DateTime.UtcNow)
                .Include(q => q.Reserva)
                .OrderByDescending(q => q.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        // Implementación de nuevos métodos para el controlador

        /// <summary>
        /// Alias para GetQRPorHashAsync - mantiene compatibilidad
        /// </summary>
        public async Task<QRLink?> GetByHashAsync(string hash)
        {
            return await GetQRPorHashAsync(hash);
        }

        /// <summary>
        /// Crea un nuevo QR Link para una reserva específica
        /// ¿Por qué método separado? Encapsula la lógica de creación con valores por defecto
        /// </summary>
        public async Task<QRLink> CreateQRLinkAsync(int reservaId)
        {
            var qrLink = new QRLink
            {
                Hash = Guid.NewGuid().ToString("N")[..16], // 16 caracteres únicos
                ReservaId = reservaId,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(10), // 10 minutos de validez
                Usado = false,
                Accion = "VerDetalle"
            };

            return await CrearQRAsync(qrLink);
        }

        /// <summary>
        /// Invalida todos los QRs existentes para una reserva
        /// ¿Por qué invalidar? Para que solo haya un QR válido por reserva a la vez
        /// </summary>
        public async Task InvalidateByReservaIdAsync(int reservaId)
        {
            var qrsExistentes = await _context.QRLinks
                .Where(q => q.ReservaId == reservaId && !q.Usado)
                .ToListAsync();

            foreach (var qr in qrsExistentes)
            {
                qr.Usado = true; // Marcar como usado para invalidar
            }

            if (qrsExistentes.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Limpia QRs expirados y retorna la cantidad eliminada
        /// ¿Por qué retornar cantidad? Para auditoría y logs
        /// </summary>
        public async Task<int> CleanupExpiredAsync()
        {
            var fechaLimite = DateTime.UtcNow;

            var qrsExpirados = await _context.QRLinks
                .Where(q => q.FechaExpiracion < fechaLimite)
                .ToListAsync();

            if (qrsExpirados.Any())
            {
                _context.QRLinks.RemoveRange(qrsExpirados);
                await _context.SaveChangesAsync();
            }

            return qrsExpirados.Count;
        }

        /// <summary>
        /// Elimina un QR específico por ID
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var qrLink = await _context.QRLinks.FindAsync(id);
            if (qrLink != null)
            {
                _context.QRLinks.Remove(qrLink);
                await _context.SaveChangesAsync();
            }
        }
    }
}
