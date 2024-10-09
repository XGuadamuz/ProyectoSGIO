using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProyectoSGIOCore.Services
{
    public interface IPasswordRecoveryService
    {
        Task<bool> SendRecoveryLinkAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
