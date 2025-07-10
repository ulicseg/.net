using System.ComponentModel.DataAnnotations;

namespace ReservasApp.WebAPI.Models
{
    /// <summary>
    /// Modelo para gestionar enlaces QR seguros en la API
    /// </summary>
    public class QRLink
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Hash { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime FechaExpiracion { get; set; }
        
        public bool Usado { get; set; } = false;
        
        // Relación con la reserva
        public int? ReservaId { get; set; }
        public virtual Reserva? Reserva { get; set; }
        
        // Datos adicionales del QR
        [StringLength(100)]
        public string? Accion { get; set; }
        
        [StringLength(500)]
        public string? DatosAdicionales { get; set; }
        
        // Propiedad calculada para verificar si está vigente
        public bool EstaVigente => !Usado && DateTime.UtcNow <= FechaExpiracion;
        
        // Método estático para crear un QR con expiración de 10 minutos
        public static QRLink CrearParaReserva(int reservaId, string accion = "VerDetalle")
        {
            return new QRLink
            {
                Hash = GenerarHashSeguro(),
                FechaExpiracion = DateTime.UtcNow.AddMinutes(10), // 10 minutos como pide el TP
                ReservaId = reservaId,
                Accion = accion
            };
        }
        
        private static string GenerarHashSeguro()
        {
            // Combinamos timestamp + GUID + datos aleatorios para máxima seguridad
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var guid = Guid.NewGuid().ToString("N");
            var random = new Random().Next(100000, 999999);
            
            var data = $"{timestamp}-{guid}-{random}";
            
            // Usamos SHA256 para crear un hash seguro
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hashBytes).Replace("/", "_").Replace("+", "-").Replace("=", "");
        }
    }
}
