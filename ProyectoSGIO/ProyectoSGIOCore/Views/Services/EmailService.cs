using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProyectoSGIOCore.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                _logger.LogInformation($"Configuración SMTP: Server={smtpServer}, Port={smtpPort}, SSL={enableSsl}");

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email));

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = enableSsl;

                    _logger.LogInformation($"Iniciando envío de correo a {email}...");
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Correo enviado exitosamente a {email}");
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"Error SMTP al enviar correo a {email}: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error general al enviar correo a {email}: {ex.GetType().Name} - {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
