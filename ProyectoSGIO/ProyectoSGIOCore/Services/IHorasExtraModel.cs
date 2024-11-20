using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Services
{
    public interface IHorasExtraModel
    {
        Task<HorasExtra> RegistrarHorasExtra(HorasExtra horasExtra);
        Task<HorasExtra> ObtenerHorasExtraPorId(int idHorasExtra);
        Task<List<HorasExtra>> ObtenerHorasExtraPorEmpleado(int idEmpleado, int mes, int año);
        Task<HorasExtra> ActualizarHorasExtra(HorasExtra horasExtra);
        Task<bool> EliminarHorasExtra(int idHorasExtra);
        Task<List<HorasExtra>> ObtenerHorasExtraPendientes();
        Task<List<HorasExtra>> GenerarReporteHorasExtra(int mes, int año);
    }
}
