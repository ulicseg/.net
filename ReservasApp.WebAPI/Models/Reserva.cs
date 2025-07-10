using System.ComponentModel.DataAnnotations;

namespace ReservasApp.WebAPI.Models
{
    /// <summary>
    /// Enumeración para tipos de servicio (idéntica al MVC)
    /// </summary>
    public enum TipoServicio
    {
        [Display(Name = "Consulta Médica")]
        ConsultaMedica = 1,
        
        [Display(Name = "Terapia Física")]
        TerapiaFisica = 2,
        
        [Display(Name = "Consulta Nutricional")]
        ConsultaNutricional = 3,
        
        [Display(Name = "Examen de Laboratorio")]
        ExamenLaboratorio = 4,
        
        [Display(Name = "Consulta Psicológica")]
        ConsultaPsicologica = 5,
        
        [Display(Name = "Cirugía Menor")]
        CirugiaMenor = 6
    }

    /// <summary>
    /// Estados de reserva (idénticos al MVC)
    /// </summary>
    public enum EstadoReserva
    {
        Activa = 1,
        Completada = 2,
        Cancelada = 3
    }

    /// <summary>
    /// Modelo de Reserva para la API
    /// </summary>
    public class Reserva
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        public DateTime FechaReserva { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        public TipoServicio TipoServicio { get; set; }
        
        public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;
        
        // Relación con Usuario
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        
        // Propiedad de navegación
        public virtual Usuario? Usuario { get; set; }
        
        // Propiedades calculadas para la API
        public string EstadoTexto => Estado switch
        {
            EstadoReserva.Activa => "Activa",
            EstadoReserva.Completada => "Completada",
            EstadoReserva.Cancelada => "Cancelada",
            _ => "Desconocido"
        };
        
        public string TipoServicioTexto => TipoServicio switch
        {
            TipoServicio.ConsultaMedica => "Consulta Médica",
            TipoServicio.TerapiaFisica => "Terapia Física",
            TipoServicio.ConsultaNutricional => "Consulta Nutricional",
            TipoServicio.ExamenLaboratorio => "Examen de Laboratorio",
            TipoServicio.ConsultaPsicologica => "Consulta Psicológica",
            TipoServicio.CirugiaMenor => "Cirugía Menor",
            _ => "Desconocido"
        };
    }
}
