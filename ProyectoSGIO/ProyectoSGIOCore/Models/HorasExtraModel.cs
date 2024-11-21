using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Services;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoSGIOCore.Models
{
    public class HorasExtraModel : IHorasExtraModel
    {
        private readonly AppDBContext _context;

        public HorasExtraModel(AppDBContext context)
        {
            _context = context;
        }

        public async Task<HorasExtra> RegistrarHorasExtra(HorasExtra horasExtra)
        {
            horasExtra.FechaRegistro = DateTime.Now;
            horasExtra.Estado = EstadoHorasExtra.Pendiente;

            _context.HorasExtras.Add(horasExtra);
            await _context.SaveChangesAsync();
            return horasExtra;
        }

        public async Task<HorasExtra> ObtenerHorasExtraPorId(int idHorasExtra)
        {
            return await _context.HorasExtras
                .Include(h => h.Empleado)
                .Include(h => h.Supervisor)
                .FirstOrDefaultAsync(h => h.IdHorasExtra == idHorasExtra);
        }

        public async Task<List<HorasExtra>> ObtenerHorasExtraPorEmpleado(int idEmpleado, int mes, int año)
        {
            return await _context.HorasExtras
                .Where(h => h.IdEmpleado == idEmpleado &&
                       h.Fecha.Month == mes &&
                       h.Fecha.Year == año)
                .ToListAsync();
        }

        public async Task<HorasExtra> ActualizarHorasExtra(HorasExtra horasExtra)
        {
            var horasExtraExistente = await _context.HorasExtras.FindAsync(horasExtra.IdHorasExtra);

            if (horasExtraExistente == null)
                throw new Exception("Horas extra no encontradas");

            // Actualizar propiedades
            horasExtraExistente.Fecha = horasExtra.Fecha;
            horasExtraExistente.CantidadHoras = horasExtra.CantidadHoras;
            horasExtraExistente.Descripcion = horasExtra.Descripcion;
            horasExtraExistente.TipoCompensacion = horasExtra.TipoCompensacion;

            // Si hay un supervisor, actualizar estado
            if (horasExtra.IdSupervisor.HasValue)
            {
                horasExtraExistente.IdSupervisor = horasExtra.IdSupervisor;
                horasExtraExistente.Estado = horasExtra.Estado;
            }

            await _context.SaveChangesAsync();
            return horasExtraExistente;
        }

        public async Task<bool> EliminarHorasExtra(int idHorasExtra)
        {
            var horasExtra = await _context.HorasExtras.FindAsync(idHorasExtra);

            if (horasExtra == null)
                return false;

            _context.HorasExtras.Remove(horasExtra);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HorasExtra>> ObtenerHorasExtraPendientes()
        {
            return await _context.HorasExtras
                .Where(h => h.Estado == EstadoHorasExtra.Pendiente)
                .Include(h => h.Empleado)
                .ToListAsync();
        }

        public async Task<List<HorasExtra>> GenerarReporteHorasExtra(int mes, int año)
        {
            return await _context.HorasExtras
                .Where(h => h.Fecha.Month == mes && h.Fecha.Year == año)
                .Include(h => h.Empleado)
                .ToListAsync();
        }
    }
}
