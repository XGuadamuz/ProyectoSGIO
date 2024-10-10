using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;

public class NotificacionService
{
    private readonly IEmailSender _emailSender;

    public NotificacionService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public void RevisarHorasExtraEmpleado(Empleado empleado)
    {
        if (empleado.SuperaUmbralHorasExtra())
        {
            EnviarNotificacionAdministrador(empleado);
        }
    }

    private void EnviarNotificacionAdministrador(Empleado empleado)
    {
        string mensaje = $"El empleado {empleado.Nombre} ha acumulado {empleado.HorasExtraAcumuladas} horas extra, que supera el umbral de {empleado.UmbralHorasExtra}.";
        _emailSender.SendEmailAsync("admin@empresa.com", "Umbral de horas extra superado", mensaje);
    }

    
}
