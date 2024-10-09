using System.Threading.Tasks;

namespace ProyectoSGIOCore.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
