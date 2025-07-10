using System.ComponentModel.DataAnnotations;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.DTOs
{
    /// <summary>
    /// DTOs para operaciones CRUD de reservas
    /// ¿Por qué múltiples DTOs? Porque los requerimientos de datos varían por operación
    /// </summary>

    // Para listar reservas (información mínima)
    public class ReservaListDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
    }

    // Para mostrar detalles completos
    public class ReservaDetailDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TipoServicio TipoServicio { get; set; }
        public string TipoServicioTexto { get; set; } = string.Empty;
        public EstadoReserva Estado { get; set; }
        public string EstadoTexto { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public UserDto? Usuario { get; set; }
    }

    // Para crear nuevas reservas
    public class CreateReservaDto
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        [Range(1, 6, ErrorMessage = "Seleccione un tipo de servicio válido")]
        public TipoServicio TipoServicio { get; set; }
    }

    // Para actualizar reservas existentes
    public class UpdateReservaDto
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        [Range(1, 6, ErrorMessage = "Seleccione un tipo de servicio válido")]
        public TipoServicio TipoServicio { get; set; }

        [Range(1, 3, ErrorMessage = "Seleccione un estado válido")]
        public EstadoReserva Estado { get; set; }
    }

    // Para respuestas paginadas
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // Para respuestas de operaciones
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        
        public static ApiResponseDto<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
        
        public static ApiResponseDto<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
