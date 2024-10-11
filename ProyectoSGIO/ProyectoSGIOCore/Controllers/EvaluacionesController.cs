using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProyectoSGIOCore.Models; 
using ProyectoSGIOCore.Services;
using ProyectoSGIOCore.Views.Services;


public class EvaluacionesController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IEvaluacionService _evaluacionService;
    private readonly IEmailService _emailService;
    private readonly IExportService _exportService;

    public EvaluacionesController(IEmpleadoService empleadoService, IEvaluacionService evaluacionService, IEmailService emailService, IExportService exportService)
    {
        _empleadoService = empleadoService;
        _evaluacionService = evaluacionService;
        _emailService = emailService;
        _exportService = exportService;
    }

    [HttpPost]
    public async Task<IActionResult> AsignarMetas(int empleadoId, EvaluacionDesempeño evaluacion)
    {
        // 1. Obtener el empleado
        var empleado = await _empleadoService.ObtenerEmpleadoPorId(empleadoId);
        if (empleado == null)
        {
            return NotFound("Empleado no encontrado.");
        }

        // 2. Validar metas
        if (string.IsNullOrWhiteSpace(evaluacion.Metas))
        {
            return BadRequest("Las metas no pueden estar vacías.");
        }

        // 3. Guardar evaluación
        await _evaluacionService.GuardarEvaluacion(empleadoId, evaluacion);

        // 4. Notificar al empleado
        await _emailService.SendEmailAsync(empleado.Correo, "Metas Asignadas", "Se te han asignado nuevas metas para tu evaluación.");

        return Ok("Metas asignadas y empleado notificado.");
    }

    public async Task<IActionResult> EvaluarDesempeno(int empleadoId, EvaluacionDesempeño evaluacion)
    {
        
        var empleado = await _empleadoService.ObtenerEmpleadoPorId(empleadoId);
        if (empleado == null)
        {
            return NotFound("Empleado no encontrado.");
        }

        // Guardar evaluación
        await _evaluacionService.GuardarEvaluacion(empleadoId, evaluacion);

        // Notificar al empleado
        await _emailService.SendEmailAsync(empleado.Correo, "Evaluación de Desempeño", "Tu evaluación ha sido completada.");

        return Ok("Evaluación guardada y empleado notificado.");
    }

    public async Task<IActionResult> ObtenerEvaluacionesPrevias(int empleadoId)
    {
        var evaluaciones = await _evaluacionService.ObtenerEvaluacionesPorEmpleado(empleadoId);
        if (evaluaciones == null || !evaluaciones.Any())
        {
            return NotFound("No se encontraron evaluaciones para este empleado.");
        }

        return Ok(evaluaciones);
    }

    public async Task<IActionResult> EditarEvaluacion(int evaluacionId, EvaluacionDesempeño nuevaEvaluacion)
    {
        var evaluacionExistente = await _evaluacionService.ObtenerEvaluacionPorId(evaluacionId);
        if (evaluacionExistente == null)
        {
            return NotFound("Evaluación no encontrada.");
        }

        // Actualizar criterios de evaluación
        evaluacionExistente.Criterios = nuevaEvaluacion.Criterios;
        await _evaluacionService.ActualizarEvaluacion(evaluacionExistente);

        return Ok("Evaluación actualizada correctamente.");
    }

    public async Task<IActionResult> GenerarReporteDesempeno(DateTime fechaInicio, DateTime fechaFin)
    {
        var reportes = await _evaluacionService.GenerarReporteDesempeno(fechaInicio, fechaFin);

        // Lógica para exportar el reporte a PDF/Excel
        var archivoGenerado = await _exportService.ExportarReporte(reportes, "PDF");

        return File(archivoGenerado, "application/pdf", "ReporteDesempeno.pdf");
    }

    public async Task<IActionResult> RevisarEvaluacionesPendientes()
    {
        var evaluacionesPendientes = await _evaluacionService.ObtenerEvaluacionesPendientes();
        foreach (var evaluacion in evaluacionesPendientes)
        {
            await _emailService.SendEmailAsync(evaluacion.AdministradorEmail, "Evaluación Pendiente", "Tienes una evaluación pendiente que necesita ser completada.");
        }

        return Ok("Notificaciones enviadas para evaluaciones pendientes.");
    }

}

