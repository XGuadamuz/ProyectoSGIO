using ProyectoSGIOCore.Models;
namespace ProyectoSGIOCore.Services
{
    public class HorasExtraService : IHorasExtraService
    {
        private readonly IHorasExtraModel _horasExtraModel;
        private readonly ILogger<HorasExtraService> _logger;
        public HorasExtraService(
        IHorasExtraModel horasExtraRepository,
        ILogger<HorasExtraService> logger)
        {
            _horasExtraModel = horasExtraRepository;
            _logger = logger;
        }
        public async Task<HorasExtra> RegistrarHorasExtra(HorasExtra horasExtra)
        {
            try
            {
                // Validaciones de negocio
                if (horasExtra.CantidadHoras <= 0)
                    throw new ArgumentException("La cantidad de horas debe ser mayor a cero");
                if (horasExtra.Fecha > DateTime.Now)
                    throw new ArgumentException("No se pueden registrar horas extra futuras");
                return await _horasExtraModel.RegistrarHorasExtra(horasExtra);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar horas extra: {ex.Message}");
                throw;
            }
        }
        public async Task<HorasExtra> AprobarHorasExtra(int idHorasExtra, int idSupervisor)
        {
            try
            {
                var horasExtra = await _horasExtraModel.ObtenerHorasExtraPorId(idHorasExtra);
                if (horasExtra == null)
                    throw new ArgumentException("Registro de horas extra no encontrado");
                horasExtra.IdSupervisor = idSupervisor;
                horasExtra.Estado = EstadoHorasExtra.Aprobado;
                return await _horasExtraModel.ActualizarHorasExtra(horasExtra);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al aprobar horas extra: {ex.Message}");
                throw;
            }
        }
        public async Task<HorasExtra> RechazarHorasExtra(int idHorasExtra, int idSupervisor, string motivoRechazo)
        {
            try
            {
                var horasExtra = await _horasExtraModel.ObtenerHorasExtraPorId(idHorasExtra);
                if (horasExtra == null)
                    throw new ArgumentException("Registro de horas extra no encontrado");
                horasExtra.IdSupervisor = idSupervisor;
                horasExtra.Estado = EstadoHorasExtra.Rechazado;
                horasExtra.Descripcion += $" - Motivo de rechazo: {motivoRechazo}";
                return await _horasExtraModel.ActualizarHorasExtra(horasExtra);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al rechazar horas extra: {ex.Message}");
                throw;
            }
        }
        public async Task<List<HorasExtra>> ObtenerHorasExtraMensualesPorEmpleado(int idEmpleado, int mes, int año)
        {
            try
            {
                return await _horasExtraModel.ObtenerHorasExtraPorEmpleado(idEmpleado, mes, año);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener horas extra mensuales: {ex.Message}");
                throw;
            }
        }
        public async Task<ReporteHorasExtra> GenerarReporteMensual(int mes, int año)
        {
            try
            {
                var horasExtras = await _horasExtraModel.GenerarReporteHorasExtra(mes, año);
                return new ReporteHorasExtra
                {
                    Mes = mes,
                    Año = año,
                    TotalHorasExtras = horasExtras.Sum(h => h.CantidadHoras),
                    DetalleHorasExtras = horasExtras.GroupBy(h => h.IdEmpleado)
                        .Select(g => new DetalleHorasExtraEmpleado
                        {
                            IdEmpleado = g.Key,
                            NombreEmpleado = g.First().Empleado.Nombre + " " + g.First().Empleado.Apellido,
                            TotalHorasExtras = g.Sum(h => h.CantidadHoras),
                            HorasExtrasDetalle = g.ToList()
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte mensual: {ex.Message}");
                throw;
            }
        }
    }
    // DTOs para manejo de reportes
    public class ReporteHorasExtra
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public int TotalHorasExtras { get; set; }
        public List<DetalleHorasExtraEmpleado> DetalleHorasExtras { get; set; }
    }
    public class DetalleHorasExtraEmpleado
    {
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public int TotalHorasExtras { get; set; }
        public List<HorasExtra> HorasExtrasDetalle { get; set; }
    }
}
