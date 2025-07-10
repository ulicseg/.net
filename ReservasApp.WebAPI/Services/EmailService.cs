using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ReservasApp.WebAPI.Services
{
    /// <summary>
    /// Configuración de email para la API
    /// </summary>
    public class EmailSettings
    {
        public bool SimularEnvio { get; set; } = true;
        public string From { get; set; } = "sistemagestorlegislativo@gmail.com";
        public string FromName { get; set; } = "Sistema de Reservas";
        public SmtpSettings SMTP { get; set; } = new();
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = "sistemagestorlegislativo@gmail.com";
        public string Password { get; set; } = string.Empty; // Se configurará desde appsettings
    }

    /// <summary>
    /// Implementación del servicio de email para la API
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string callbackUrl)
        {
            var subject = "Restablecer Contraseña - Sistema de Reservas";
            var body = GeneratePasswordResetEmailBody(resetToken, callbackUrl);

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendEmailConfirmationAsync(string email, string confirmationToken, string callbackUrl)
        {
            var subject = "Confirmar Email - Sistema de Reservas";
            var body = GenerateEmailConfirmationBody(confirmationToken, callbackUrl);

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string userName)
        {
            var subject = "¡Bienvenido al Sistema de Reservas!";
            var body = GenerateWelcomeEmailBody(userName);

            return await SendEmailAsync(email, subject, body);
        }

        private async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Si está en modo simulación, solo loggear
                if (_emailSettings.SimularEnvio)
                {
                    _logger.LogInformation("📧 SIMULACIÓN DE EMAIL");
                    _logger.LogInformation("Para: {Email}", to);
                    _logger.LogInformation("Asunto: {Subject}", subject);
                    _logger.LogInformation("Contenido: {Body}", body);
                    return true;
                }

                // Envío real
                using var client = new SmtpClient(_emailSettings.SMTP.Host, _emailSettings.SMTP.Port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(
                    _emailSettings.SMTP.Username, 
                    _emailSettings.SMTP.Password);

                using var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.From, _emailSettings.FromName);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);

                _logger.LogInformation("✅ Email enviado exitosamente a {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando email a {Email}", to);
                return false;
            }
        }

        private string GeneratePasswordResetEmailBody(string resetToken, string callbackUrl)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2563eb;'>Restablecer Contraseña</h2>
                    <p>Has solicitado restablecer tu contraseña en el Sistema de Reservas.</p>
                    <p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
                    <div style='margin: 20px 0;'>
                        <a href='{callbackUrl}' 
                           style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 6px; display: inline-block;'>
                            Restablecer Contraseña
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>
                        Si no solicitaste este cambio, puedes ignorar este correo de forma segura.
                    </p>
                    <p style='color: #666; font-size: 14px;'>
                        Token de recuperación: <code>{resetToken}</code>
                    </p>
                    <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>
                        Sistema de Reservas - Programación 3<br>
                        Este enlace expirará en 24 horas.
                    </p>
                </div>";
        }

        private string GenerateEmailConfirmationBody(string confirmationToken, string callbackUrl)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #16a34a;'>Confirmar Email</h2>
                    <p>¡Gracias por registrarte en el Sistema de Reservas!</p>
                    <p>Para completar tu registro, confirma tu dirección de email:</p>
                    <div style='margin: 20px 0;'>
                        <a href='{callbackUrl}' 
                           style='background-color: #16a34a; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 6px; display: inline-block;'>
                            Confirmar Email
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>
                        Token de confirmación: <code>{confirmationToken}</code>
                    </p>
                    <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>
                        Sistema de Reservas - Programación 3
                    </p>
                </div>";
        }

        private string GenerateWelcomeEmailBody(string userName)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #7c3aed;'>¡Bienvenido, {userName}!</h2>
                    <p>Tu cuenta ha sido creada exitosamente en el Sistema de Reservas.</p>
                    <p>Ya puedes comenzar a gestionar tus reservas:</p>
                    <ul style='color: #666;'>
                        <li>Crear nuevas reservas</li>
                        <li>Ver tu historial</li>
                        <li>Generar códigos QR</li>
                        <li>Gestionar tus datos</li>
                    </ul>
                    <div style='margin: 20px 0;'>
                        <a href='https://localhost:7092' 
                           style='background-color: #7c3aed; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 6px; display: inline-block;'>
                            Acceder al Sistema
                        </a>
                    </div>
                    <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>
                        Sistema de Reservas - Programación 3
                    </p>
                </div>";
        }
    }
}
