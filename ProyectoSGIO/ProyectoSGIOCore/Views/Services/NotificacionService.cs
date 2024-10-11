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
        // Revisa si el empleado ha superado su umbral de horas extra
        if (empleado.SuperaUmbralHorasExtra())
        {
            EnviarNotificacionAdministrador(empleado);
            EnviarNotificacionEmpleado(empleado);
        }
    }

    // Método para notificar al administrador cuando se supera el umbral de horas extra
    private void EnviarNotificacionAdministrador(Empleado empleado)
    {
        string mensaje = $"El empleado {empleado.Nombre} ha acumulado {empleado.HorasExtraAcumuladas} horas extra, que supera el umbral de {empleado.UmbralHorasExtra}.";
        _emailSender.SendEmailAsync("admin@empresa.com", "Umbral de horas extra superado", mensaje);
    }

    // Método para notificar al empleado que ha acumulado demasiadas horas extra
    private void EnviarNotificacionEmpleado(Empleado empleado)
    {
        string mensaje = $"Has acumulado {empleado.HorasExtraAcumuladas} horas extra. Superas el umbral de {empleado.UmbralHorasExtra}. Por favor contacta con tu supervisor.";
        _emailSender.SendEmailAsync(empleado.Correo, "Acumulación de horas extra", mensaje);
    }
}

