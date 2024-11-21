using ProyectoSGIOCore.Models;
namespace ProyectoSGIOCore.Services
{
    public interface IHorasExtraService
    {
        Task<HorasExtra> RegistrarHorasExtra(HorasExtra horasExtra);
        Task<HorasExtra> AprobarHorasExtra(int idHorasExtra, int idSupervisor);
        Task<HorasExtra> RechazarHorasExtra(int idHorasExtra, int idSupervisor, string motivoRechazo);
        Task<List<HorasExtra>> ObtenerHorasExtraMensualesPorEmpleado(int idEmpleado, int mes, int año);
        Task<ReporteHorasExtra> GenerarReporteMensual(int mes, int año);
    }
}
