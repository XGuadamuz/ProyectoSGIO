using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Views.Services
{
    public interface IEmpleadoService
    {
        Task<Empleado> ObtenerEmpleadoPorId(int empleadoId);
    }
}
