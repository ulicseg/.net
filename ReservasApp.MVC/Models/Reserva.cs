using System.ComponentModel.DataAnnotations;

namespace ReservasApp.MVC.Models
{
    /// <summary>
    /// Enumeración para tipos de servicio
    /// ¿Por qué un enum? Porque garantiza valores consistentes y es fácil de usar en dropdowns
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
    /// Modelo principal de Reserva
    /// Esta es nuestra entidad principal para el CRUD
    /// </summary>
    public class Reserva
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        [Display(Name = "Título de la Reserva")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        [Display(Name = "Fecha de Reserva")]
        [DataType(DataType.DateTime)]
        public DateTime FechaReserva { get; set; }
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        [Display(Name = "Tipo de Servicio")]
        public TipoServicio TipoServicio { get; set; }
        
        [Display(Name = "Estado")]
        public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;
        
        // Relación con Usuario
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        
        // Propiedad de navegación
        public virtual Usuario? Usuario { get; set; }
        
        // Propiedad calculada para mostrar en las vistas
        [Display(Name = "Estado de la Reserva")]
        public string EstadoTexto => Estado switch
        {
            EstadoReserva.Activa => "Activa",
            EstadoReserva.Completada => "Completada",
            EstadoReserva.Cancelada => "Cancelada",
            _ => "Desconocido"
        };
    }
    
    /// <summary>
    /// Estados posibles de una reserva
    /// </summary>
    public enum EstadoReserva
    {
        Activa = 1,
        Completada = 2,
        Cancelada = 3
    }
}
