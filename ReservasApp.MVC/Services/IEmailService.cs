namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de email
    /// ¿Por qué una interfaz? Para poder usar diferentes proveedores (SMTP, SendGrid, etc.)
    /// </summary>
    public interface IEmailService
    {
        Task<bool> EnviarEmailRecuperacionAsync(string email, string token, string urlCallback);
        Task<bool> EnviarEmailConfirmacionAsync(string email, string token, string urlCallback);
        Task<bool> EnviarEmailNotificacionAsync(string email, string asunto, string mensaje);
    }
}
