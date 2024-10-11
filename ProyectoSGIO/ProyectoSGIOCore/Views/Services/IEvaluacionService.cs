using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoSGIOCore.Models; // Asegúrate de que esto apunte al namespace correcto

public interface IEvaluacionService
{
    Task GuardarEvaluacion(int empleadoId, EvaluacionDesempeño evaluacion);
    Task<EvaluacionDesempeño> ObtenerEvaluacionPorId(int evaluacionId);
    Task ActualizarEvaluacion(EvaluacionDesempeño evaluacion);
    Task<List<EvaluacionDesempeño>> ObtenerEvaluacionesPorEmpleado(int empleadoId);
    Task<List<EvaluacionDesempeño>> GenerarReporteDesempeno(DateTime fechaInicio, DateTime fechaFin);
    Task<List<EvaluacionDesempeño>> ObtenerEvaluacionesPendientes();
}