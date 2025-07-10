using ReservasApp.MVC.Models;

namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de reservas
    /// ¿Por qué una interfaz? Para poder hacer mocking en tests y seguir el principio de inversión de dependencias
    /// </summary>
    public interface IReservaService
    {
        Task<IEnumerable<Reserva>> GetReservasPaginadasAsync(string usuarioId, int pagina = 1, int tamañoPagina = 10);
        Task<Reserva?> GetReservaPorIdAsync(int id, string usuarioId);
        Task<Reserva> CrearReservaAsync(Reserva reserva);
        Task<bool> ActualizarReservaAsync(Reserva reserva, string usuarioId);
        Task<bool> EliminarReservaAsync(int id, string usuarioId);
        Task<int> GetTotalReservasAsync(string usuarioId);
        Task<IEnumerable<Reserva>> GetReservasRecientesAsync(string usuarioId, int cantidad = 5);
    }
}
