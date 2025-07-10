namespace ReservasApp.WebAPI.Services
{
    /// <summary>
    /// Interfaz para el servicio de email en la API
    /// </summary>
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string callbackUrl);
        Task<bool> SendEmailConfirmationAsync(string email, string confirmationToken, string callbackUrl);
        Task<bool> SendWelcomeEmailAsync(string email, string userName);
    }
}
