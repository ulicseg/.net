namespace ReservasApp.WebAPI.DTOs
{
    /// <summary>
    /// DTOs para operaciones con códigos QR
    /// ¿Por qué DTOs específicos? Para controlar la información sensible de los QR
    /// </summary>

    // Para crear un QR
    public class CreateQRDto
    {
        public int ReservaId { get; set; }
        public string Accion { get; set; } = "VerDetalle";
        public string? DatosAdicionales { get; set; }
    }

    // Para generar códigos QR (usado en controlador)
    public class QRGenerateRequestDto
    {
        public int ReservaId { get; set; }
    }

    public class QRGenerateResponseDto
    {
        public string Hash { get; set; } = string.Empty;
        public string QRUrl { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public int ReservaId { get; set; }
        public int MinutosValidez { get; set; }
    }

    // Para respuesta de QR creado (legacy)
    public class QRResponseDto
    {
        public string Hash { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public int MinutosParaExpirar { get; set; }
        public bool EstaVigente { get; set; }
    }

    // Para acceso a QR (información limitada por seguridad)
    public class QRAccessDto
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public ReservaDetailDto? Reserva { get; set; }
        public string? Accion { get; set; }
        public DateTime? FechaAcceso { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de acceso via QR
    /// ¿Por qué específico? Contiene solo información necesaria para acceso QR
    /// </summary>
    public class QRAccessResponseDto
    {
        public int ReservaId { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime FechaAcceso { get; set; }
        public string MensajeAcceso { get; set; } = string.Empty;
    }

    // Para generar imagen QR
    public class QRImageDto
    {
        public string Hash { get; set; } = string.Empty;
        public string ImageBase64 { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
    }
}
