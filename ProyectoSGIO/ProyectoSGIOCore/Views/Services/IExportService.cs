using ProyectoSGIOCore.Models;
using System.Threading.Tasks;


namespace ProyectoSGIOCore.Views.Services
{
    public interface IExportService
    {
        Task ExportarDatosAsync();
        Task<byte[]> ExportarReporte(List<EvaluacionDesempeño> reportes, string formato);
    }
}
