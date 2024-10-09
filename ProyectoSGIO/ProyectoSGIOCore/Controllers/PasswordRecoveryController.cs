using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.ViewModels;
using System.Threading.Tasks;

namespace ProyectoSGIOCore.Controllers
{
    public class PasswordRecoveryController : Controller
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;

        public PasswordRecoveryController(IPasswordRecoveryService passwordRecoveryService)
        {
            _passwordRecoveryService = passwordRecoveryService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(PasswordRecoveryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _passwordRecoveryService.SendRecoveryLinkAsync(model.Email);
            if (result)
            {
                return View("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
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