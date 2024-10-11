using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.ViewModels;
using ProyectoSGIOCore.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProyectoSGIOCore.Models;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace ProyectoSGIOCore.Controllers
{
    public class PasswordRecoveryController : Controller
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordRecoveryController> _logger;

        public PasswordRecoveryController(
            IPasswordRecoveryService passwordRecoveryService,
            UserManager<Usuario> userManager,
            IEmailService emailService,
            ILogger<PasswordRecoveryController> logger)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(PasswordRecoveryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation($"Iniciando proceso de recuperación de contraseña para: {model.Email}");

            var user = await _userManager.FindByEmailAsync(model.Email.ToUpper());

            if (user == null)
            {
                _logger.LogWarning($"No se encontró usuario para el correo: {model.Email}");
                ModelState.AddModelError(string.Empty, "No se encontró un usuario con ese correo electrónico.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "PasswordRecovery", new { token, email = model.Email }, protocol: HttpContext.Request.Scheme);

            var htmlMessage = $"<h4>Restablecer su contraseña</h4>" +
                              $"<p>Haga clic en el siguiente enlace para restablecer su contraseña:</p>" +
                              $"<a href='{callbackUrl}'>Restablecer contraseña</a>";

            try
            {
                await _emailService.SendEmailAsync(model.Email, "Restablecer contraseña", htmlMessage);
                _logger.LogInformation($"Correo de recuperación enviado exitosamente a: {model.Email}");
                return View("ForgotPasswordConfirmation");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"Error SMTP al enviar correo de recuperación: {smtpEx.Message}");
                ModelState.AddModelError(string.Empty, "No se pudo enviar el correo electrónico. Intente nuevamente más tarde.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error general al enviar correo de recuperación: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _passwordRecoveryService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}