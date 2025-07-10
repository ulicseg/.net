using Microsoft.EntityFrameworkCore;
using ReservasApp.WebAPI.Data;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de reservas
    /// ¿Por qué Repository Pattern? Para abstraer el acceso a datos y facilitar testing
    /// </summary>
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetReservasPaginadasAsync(string usuarioId, int pagina, int tamañoPagina);
        Task<Reserva?> GetReservaPorIdAsync(int id, string usuarioId);
        Task<Reserva?> GetByIdAsync(int id); // Para operaciones QR sin filtro de usuario
        Task<Reserva> CrearReservaAsync(Reserva reserva);
        Task<Reserva> ActualizarReservaAsync(Reserva reserva);
        Task<bool> EliminarReservaAsync(int id, string usuarioId);
        Task<int> GetTotalReservasAsync(string usuarioId);
        Task<bool> ExisteReservaAsync(int id, string usuarioId);
    }

    /// <summary>
    /// Implementación del repositorio de reservas
    /// ¿Por qué async? Para no bloquear threads en operaciones I/O
    /// </summary>
    public class ReservaRepository : IReservaRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene reservas paginadas con información del usuario
        /// ¿Por qué Include? Para cargar datos relacionados en una sola consulta
        /// </summary>
        public async Task<IEnumerable<Reserva>> GetReservasPaginadasAsync(string usuarioId, int pagina, int tamañoPagina)
        {
            return await _context.Reservas
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaCreacion)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .AsNoTracking() // ¿Por qué AsNoTracking? Para mejorar performance en consultas de solo lectura
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una reserva específica por ID y usuario
        /// ¿Por qué verificar usuario? Seguridad: evitar acceso a datos ajenos
        /// </summary>
        public async Task<Reserva?> GetReservaPorIdAsync(int id, string usuarioId)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Crea una nueva reserva
        /// </summary>
        public async Task<Reserva> CrearReservaAsync(Reserva reserva)
        {
            reserva.FechaCreacion = DateTime.UtcNow;
            reserva.Estado = EstadoReserva.Activa;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Recargar con datos del usuario
            await _context.Entry(reserva)
                .Reference(r => r.Usuario)
                .LoadAsync();

            return reserva;
        }

        /// <summary>
        /// Actualiza una reserva existente
        /// ¿Por qué verificar existencia? Para evitar errores y problemas de concurrencia
        /// </summary>
        public async Task<Reserva> ActualizarReservaAsync(Reserva reserva)
        {
            var existente = await _context.Reservas.FindAsync(reserva.Id);
            if (existente == null)
                throw new InvalidOperationException("Reserva no encontrada");

            // Actualizar solo campos permitidos
            existente.Titulo = reserva.Titulo;
            existente.Descripcion = reserva.Descripcion;
            existente.FechaReserva = reserva.FechaReserva;
            existente.TipoServicio = reserva.TipoServicio;
            existente.Estado = reserva.Estado;

            await _context.SaveChangesAsync();

            // Recargar con datos del usuario
            await _context.Entry(existente)
                .Reference(r => r.Usuario)
                .LoadAsync();

            return existente;
        }

        /// <summary>
        /// Elimina una reserva (verificando pertenencia al usuario)
        /// </summary>
        public async Task<bool> EliminarReservaAsync(int id, string usuarioId)
        {
            var reserva = await GetReservaPorIdAsync(id, usuarioId);
            if (reserva == null)
                return false;

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cuenta el total de reservas para paginación
        /// </summary>
        public async Task<int> GetTotalReservasAsync(string usuarioId)
        {
            return await _context.Reservas
                .CountAsync(r => r.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Obtiene una reserva por ID sin filtro de usuario (para operaciones QR)
        /// ¿Por qué sin filtro? Los QR necesitan acceso directo a la reserva
        /// </summary>
        public async Task<Reserva?> GetByIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Verifica si existe una reserva para un usuario
        /// </summary>
        public async Task<bool> ExisteReservaAsync(int id, string usuarioId)
        {
            return await _context.Reservas
                .AnyAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        }
    }
}
