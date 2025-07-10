using Microsoft.AspNetCore.Identity;

namespace ReservasApp.MVC.Models
{
    /// <summary>
    /// Modelo de Usuario extendido de IdentityUser
    /// ¿Por qué extender IdentityUser? Porque nos da automáticamente:
    /// - Hash de contraseñas con salt
    /// - Funcionalidades de autenticación
    /// - Gestión de roles y claims
    /// - Recuperación de contraseñas
    /// </summary>
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        
        // Propiedad de navegación: Un usuario puede tener muchas reservas
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        
        // Propiedad calculada para mostrar nombre completo
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
