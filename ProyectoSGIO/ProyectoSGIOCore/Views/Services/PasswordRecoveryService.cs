using ProyectoSGIOCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Web;
using System.Threading.Tasks;

namespace ProyectoSGIOCore.Services
{
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public PasswordRecoveryService(UserManager<Usuario> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<bool> SendRecoveryLinkAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // No revelamos que el usuario no existe por razones de seguridad
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var callbackUrl = $"{_configuration["AppUrl"]}/Acceso/ResetPassword?email={email}&token={encodedToken}";

            await _emailService.SendEmailAsync(email, "Recuperación de Contraseña",
                $"Por favor, restablece tu contraseña haciendo clic aquí: <a href='{callbackUrl}'>enlace</a>");

            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // No revelamos que el usuario no existe por razones de seguridad
                return IdentityResult.Failed(new IdentityError { Description = "Error al restablecer la contraseña." });
            }

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}