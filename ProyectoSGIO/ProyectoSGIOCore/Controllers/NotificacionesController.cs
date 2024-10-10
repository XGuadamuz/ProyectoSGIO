using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Services;

namespace ProyectoSGIOCore.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly IEmailService _emailService;

        public NotificacionesController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> EnviarNotificacion(string destinatario)
        {
            string asunto = "Notificación";
            string mensaje = "<h1>Este es un mensaje de prueba</h1>";

            await _emailService.SendEmailAsync(destinatario, asunto, mensaje);

            return Ok("Correo enviado");
        }
    }
}
