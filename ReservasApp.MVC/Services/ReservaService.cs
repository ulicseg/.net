using Microsoft.EntityFrameworkCore;
using ReservasApp.MVC.Data;
using ReservasApp.MVC.Models;

namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Servicio para manejar operaciones CRUD de reservas
    /// ¿Por qué un servicio? Para encapsular la lógica de negocio y mantener los controladores limpios
    /// </summary>
    public class ReservaService : IReservaService
    {
        private readonly ApplicationDbContext _context;

        public ReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene reservas paginadas para un usuario específico
        /// ¿Por qué paginación? Para no cargar miles de registros y mejorar el rendimiento
        /// </summary>
        public async Task<IEnumerable<Reserva>> GetReservasPaginadasAsync(string usuarioId, int pagina = 1, int tamañoPagina = 10)
        {
            return await _context.Reservas
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Usuario) // Incluir datos del usuario para mostrar
                .OrderByDescending(r => r.FechaCreacion) // Más recientes primero
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una reserva específica verificando que pertenezca al usuario
        /// ¿Por qué verificar el usuario? Seguridad: un usuario no debe ver reservas de otros
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
            
            return reserva;
        }

        /// <summary>
        /// Actualiza una reserva existente
        /// ¿Por qué verificar el usuario? Para evitar que alguien modifique reservas ajenas
        /// </summary>
        public async Task<bool> ActualizarReservaAsync(Reserva reserva, string usuarioId)
        {
            var reservaExistente = await GetReservaPorIdAsync(reserva.Id, usuarioId);
            if (reservaExistente == null)
                return false;

            // Actualizar solo los campos permitidos
            reservaExistente.Titulo = reserva.Titulo;
            reservaExistente.Descripcion = reserva.Descripcion;
            reservaExistente.FechaReserva = reserva.FechaReserva;
            reservaExistente.TipoServicio = reserva.TipoServicio;
            reservaExistente.Estado = reserva.Estado;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina una reserva (soft delete o hard delete según necesidades)
        /// </summary>
        public async Task<bool> EliminarReservaAsync(int id, string usuarioId)
        {
            var reserva = await GetReservaPorIdAsync(id, usuarioId);
            if (reserva == null)
                return false;

            // Opción 1: Hard delete (eliminar físicamente)
            _context.Reservas.Remove(reserva);
            
            // Opción 2: Soft delete (marcar como eliminada)
            // reserva.Estado = EstadoReserva.Cancelada;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cuenta total de reservas para paginación
        /// </summary>
        public async Task<int> GetTotalReservasAsync(string usuarioId)
        {
            return await _context.Reservas
                .CountAsync(r => r.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Obtiene las reservas más recientes para dashboard
        /// </summary>
        public async Task<IEnumerable<Reserva>> GetReservasRecientesAsync(string usuarioId, int cantidad = 5)
        {
            return await _context.Reservas
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaCreacion)
                .Take(cantidad)
                .ToListAsync();
        }
    }
}
