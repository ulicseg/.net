using System.Net;
using System.Net.Mail;

namespace ReservasApp.MVC.Services
{
    /// <summary>
    /// Servicio para envío de emails
    /// ¿Por qué un servicio de email? Para recuperación de contraseñas y notificaciones
    /// En desarrollo simularemos el envío, en producción usarías un proveedor real
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Envía email de recuperación de contraseña
        /// ¿Por qué este método? Requisito del TP: recuperación por email con enlace único
        /// </summary>
        public async Task<bool> EnviarEmailRecuperacionAsync(string email, string token, string urlCallback)
        {
            var subject = "Recuperación de Contraseña - Sistema de Reservas";
            var body = $@"
                <h2>Recuperación de Contraseña</h2>
                <p>Has solicitado recuperar tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
                <a href='{urlCallback}?token={token}&email={email}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                    Restablecer Contraseña
                </a>
                <p><strong>Este enlace expira en 1 hora por seguridad.</strong></p>
                <p>Si no solicitaste este cambio, ignora este email.</p>
                <hr>
                <p><small>Sistema de Reservas - Programación 3</small></p>
            ";

            return await EnviarEmailAsync(email, subject, body);
        }

        /// <summary>
        /// Envía email de confirmación de cuenta
        /// </summary>
        public async Task<bool> EnviarEmailConfirmacionAsync(string email, string token, string urlCallback)
        {
            var subject = "Confirma tu cuenta - Sistema de Reservas";
            var body = $@"
                <h2>¡Bienvenido al Sistema de Reservas!</h2>
                <p>Gracias por registrarte.</p>
                <p>Para completar tu registro, confirma tu email haciendo clic en el enlace:</p>
                <a href='{urlCallback}?token={token}&email={email}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                    Confirmar Email
                </a>
                <p>Una vez confirmado, podrás acceder a todas las funcionalidades.</p>
                <hr>
                <p><small>Sistema de Reservas - Programación 3</small></p>
            ";

            return await EnviarEmailAsync(email, subject, body);
        }

        /// <summary>
        /// Envía email de notificación general
        /// </summary>
        public async Task<bool> EnviarEmailNotificacionAsync(string email, string asunto, string mensaje)
        {
            return await EnviarEmailAsync(email, asunto, mensaje);
        }

        /// <summary>
        /// Método base para envío de emails
        /// En desarrollo: simula el envío logueando el contenido
        /// En producción: implementa el envío real via SMTP
        /// </summary>
        private async Task<bool> EnviarEmailAsync(string email, string subject, string body)
        {
            try
            {
                // En desarrollo: simular envío logueando
                if (_configuration.GetValue<bool>("Email:SimularEnvio", true))
                {
                    _logger.LogInformation($"""
                        ====== EMAIL SIMULADO ======
                        Para: {email}
                        Asunto: {subject}
                        Contenido: {body}
                        ============================
                        """);
                    
                    await Task.Delay(500); // Simular latencia de red
                    return true;
                }

                // En producción: envío real via SMTP
                return await EnviarEmailSMTPAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email a {Email}", email);
                return false;
            }
        }

        /// <summary>
        /// Implementación real de envío SMTP (para producción)
        /// ¿Por qué separado? Para poder activar/desactivar según el ambiente
        /// </summary>
        private async Task<bool> EnviarEmailSMTPAsync(string email, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SMTP:Host"];
                var smtpPort = _configuration.GetValue<int>("Email:SMTP:Port", 587);
                var smtpUser = _configuration["Email:SMTP:Username"];
                var smtpPass = _configuration["Email:SMTP:Password"];
                var fromEmail = _configuration["Email:From"];
                var fromName = _configuration["Email:FromName"] ?? "Sistema de Reservas";

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
                {
                    _logger.LogWarning("Configuración SMTP incompleta. Simulando envío.");
                    return true;
                }

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? smtpUser, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(email);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email enviado exitosamente a {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en envío SMTP a {Email}", email);
                return false;
            }
        }
    }
}
