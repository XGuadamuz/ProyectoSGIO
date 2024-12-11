using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ProyectoSGIOCore.Services
{
    public class SmtpEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        public SmtpEmailSender(IOptions<SmtpSettings> smtpSettings) 
        { 
            _smtpSettings = smtpSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message) 
        {
            var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port) 
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true 
            };

            var mailMessage = new MailMessage 
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject, Body = message,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(email);
            await client.SendMailAsync(mailMessage);
        }
    }
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
