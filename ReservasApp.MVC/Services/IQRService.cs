using ReservasApp.MVC.Models;

namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de códigos QR
    /// ¿Por qué separar QR en su propio servicio? Porque puede tener lógica compleja de generación y validación
    /// </summary>
    public interface IQRService
    {
        Task<QRLink> CrearQRParaReservaAsync(int reservaId, string accion = "VerDetalle");
        Task<QRLink?> ValidarQRAsync(string hash);
        string GenerarImagenQR(string url);
        Task<bool> MarcarQRComoUsadoAsync(string hash);
        Task LimpiarQRsExpiradosAsync();
    }
}
