using Microsoft.AspNetCore.Identity;

namespace ReservasApp.WebAPI.Models
{
    /// <summary>
    /// Modelo de Usuario para la API (mismo que en MVC)
    /// ¿Por qué reutilizar? Para mantener consistencia entre MVC y API
    /// </summary>
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        
        // Propiedad de navegación
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        
        // Propiedad calculada
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
