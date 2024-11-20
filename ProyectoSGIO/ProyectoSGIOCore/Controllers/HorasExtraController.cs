using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.Services;
using System.Security.Claims;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HorasExtraController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<HorasExtraController> _logger;
        private readonly IHorasExtraService _horasExtraService;

        public HorasExtraController(AppDBContext dbContext, ILogger<HorasExtraController> logger, IHorasExtraService horasExtraService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _horasExtraService = horasExtraService;
        }

        // Vista para registrar horas extra
        public async Task<IActionResult> RegistrarHorasExtra()
        {
            try
            {
                // Obtener lista de empleados para el dropdown
                ViewBag.Empleados = await _dbContext.Empleados
                                           .ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar vista de registro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarHorasExtra(HorasExtra horasExtra)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Empleados = await _dbContext.Empleados
                                           .ToListAsync();
                    return View(horasExtra);
                }

                // Obtener ID del usuario actual
                horasExtra.IdEmpleado = ObtenerIdEmpleadoActual();

                var resultado = await _horasExtraService.RegistrarHorasExtra(horasExtra);

                TempData["Mensaje"] = "Horas extra registradas correctamente";
                return RedirectToAction("ListarHorasExtra");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar horas extra: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al registrar las horas extra");
                ViewBag.Empleados = await _dbContext.Empleados
                                           .ToListAsync();
                return View(horasExtra);
            }
        }
        /*
        // Vista para listar horas extra pendientes de aprobación
        public async Task<IActionResult> HorasExtraPendientes()
        {
            try
            {
                var horasExtraPendientes = await _horasExtraService.ObtenerHorasExtraPendientes();
                return View(horasExtraPendientes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener horas extra pendientes: {ex.Message}");
                return View("Error");
            }
        }*/

        // Acción para aprobar horas extra
        [HttpPost]
        public async Task<IActionResult> AprobarHorasExtra(int idHorasExtra)
        {
            try
            {
                int idSupervisor = ObtenerIdUsuarioActual();
                await _horasExtraService.AprobarHorasExtra(idHorasExtra, idSupervisor);

                TempData["Mensaje"] = "Horas extra aprobadas correctamente";
                return RedirectToAction("HorasExtraPendientes");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al aprobar horas extra: {ex.Message}");
                TempData["Error"] = "No se pudo aprobar las horas extra";
                return RedirectToAction("HorasExtraPendientes");
            }
        }

        // Acción para rechazar horas extra
        [HttpPost]
        public async Task<IActionResult> RechazarHorasExtra(int idHorasExtra, string motivoRechazo)
        {
            try
            {
                int idSupervisor = ObtenerIdUsuarioActual();
                await _horasExtraService.RechazarHorasExtra(idHorasExtra, idSupervisor, motivoRechazo);

                TempData["Mensaje"] = "Horas extra rechazadas correctamente";
                return RedirectToAction("HorasExtraPendientes");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al rechazar horas extra: {ex.Message}");
                TempData["Error"] = "No se pudo rechazar las horas extra";
                return RedirectToAction("HorasExtraPendientes");
            }
        }

        // Generar reporte mensual
        public async Task<IActionResult> GenerarReporteMensual(int? mes, int? año)
        {
            try
            {
                // Si no se especifica, usar mes y año actual
                mes ??= DateTime.Now.Month;
                año ??= DateTime.Now.Year;

                var reporte = await _horasExtraService.GenerarReporteMensual(mes.Value, año.Value);
                return View(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte mensual: {ex.Message}");
                return View("Error");
            }
        }

        // Exportar reporte a Excel
        public async Task<IActionResult> ExportarReporteExcel(int mes, int año)
        {
            try
            {
                var reporte = await _horasExtraService.GenerarReporteMensual(mes, año);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Reporte Horas Extra");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "Empleado";
                    worksheet.Cell(1, 2).Value = "Total Horas Extra";

                    // Datos
                    int fila = 2;
                    foreach (var detalle in reporte.DetalleHorasExtras)
                    {
                        worksheet.Cell(fila, 1).Value = detalle.NombreEmpleado;
                        worksheet.Cell(fila, 2).Value = detalle.TotalHorasExtras;
                        fila++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"ReporteHorasExtra_{mes}_{año}.xlsx"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al exportar reporte: {ex.Message}");
                return View("Error");
            }
        }

        // Métodos privados de utilidad
        private int ObtenerIdEmpleadoActual()
        {
            // Lógica para obtener ID de empleado del usuario actual
            // Depende de la implementación de autenticación
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        private int ObtenerIdUsuarioActual()
        {
            // Lógica para obtener ID de usuario del usuario actual
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

    }
}
