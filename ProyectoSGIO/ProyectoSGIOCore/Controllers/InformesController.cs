using IronPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.Services;
using System.Security.Claims;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador, Supervisor")]
    public class InformesController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IUtilitariosModel _utilitarios;
        public InformesController(AppDBContext dbContext, IUtilitariosModel utilitarios)
        {
            _dbContext = dbContext;
            _utilitarios = utilitarios;
        }

        [HttpGet]
        public async Task<IActionResult> GenerarInforme()
        {
            var proyectos = await _dbContext.Proyectos.ToListAsync();
            var model = new InformeProgreso
            {
                Proyectos = proyectos
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerarInforme(int proyectoId, string formato)
        {
            try
            {
                // Obtener datos del proyecto
                var proyecto = await _dbContext.Proyectos
                    .Include(p => p.Fases)
                    .ThenInclude(f => f.Tareas)
                    .Include(p => p.Hitos)
                    .ThenInclude(h => h.Usuario)
                    .FirstOrDefaultAsync(p => p.Id == proyectoId);

                if (proyecto == null)
                {
                    ViewData["Mensaje"] = "No hay datos suficientes para generar el informe.";
                    return View("Error");
                }

                // Calcular el progreso total del proyecto
                var totalTareas = proyecto.Fases.SelectMany(f => f.Tareas).Count();
                var tareasCompletadas = proyecto.Fases.SelectMany(f => f.Tareas).Count(t => t.Completada);
                var progresoGeneral = totalTareas == 0 ? 0 : (tareasCompletadas * 100 / totalTareas);

                var informe = new InformeProgreso
                {
                    Proyecto = proyecto.Nombre,
                    Supervisor = User.Identity.Name,
                    Fecha = DateTime.Now,
                    Estado = proyecto.Estado.ToString(),
                    TareasCompletadas = proyecto.Fases
                        .SelectMany(f => f.Tareas)
                        .Where(t => t.Completada)
                        .ToList(),
                    ProximasTareas = proyecto.Fases
                        .SelectMany(f => f.Tareas)
                        .Where(t => !t.Completada)
                        .ToList(),
                    Incidencias = await _dbContext.Hitos
                        .Where(h => h.ProyectoId == proyecto.Id)
                        .Select(h => h.Descripcion)
                        .ToListAsync()
                };

                // Generar informe en el formato solicitado
                IActionResult resultado;
                switch (formato.ToLower())
                {
                    case "pdf":
                        resultado = GenerarInformePDF(informe, progresoGeneral);
                        break;
                    case "csv":
                        resultado = GenerarInformeCSV(informe, progresoGeneral);
                        break;
                    case "xlsx":
                        resultado = GenerarInformeXLSX(informe, progresoGeneral);
                        break;
                    default:
                        ViewData["Mensaje"] = "Formato no soportado.";
                        return View("Error");
                }

                // Enviar notificación por correo
                var correoSupervisor = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                _utilitarios.EnviarCorreo(correoSupervisor, "Informe de Progreso Generado", "Su informe de progreso ha sido generado" +
                    " y está disponible para su descarga.");
                return resultado;
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = $"Error al generar el informe: {ex.Message}";
                return View("Error");
            }
        }

        private IActionResult GenerarInformePDF(InformeProgreso informe, int progresoGeneral)
        {
            var html = RenderizarInformeHTML(informe, progresoGeneral);
            var Renderer = new IronPdf.HtmlToPdf();
            var pdf = Renderer.RenderHtmlAsPdf(html);
            var pdfBytes = pdf.BinaryData;
            return File(pdfBytes, "application/pdf", "InformeProgreso.pdf");
        }

        private IActionResult GenerarInformeCSV(InformeProgreso informe, int progresoGeneral)
        {
            var csv = "Proyecto,Supervisor,Fecha,Estado,Progreso General\n";
            csv += $"{informe.Proyecto},{informe.Supervisor},{informe.Fecha},{informe.Estado},{progresoGeneral}%\n";
            csv += "Tareas Completadas\n";
            foreach (var tarea in informe.TareasCompletadas)
            {
                csv += $"{tarea.Nombre},{tarea.FechaInicio},{tarea.FechaFin}\n";
            }
            csv += "Próximas Tareas\n";
            foreach (var tarea in informe.ProximasTareas)
            {
                csv += $"{tarea.Nombre},{tarea.FechaInicio},{tarea.FechaFin}\n";
            }
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(csvBytes, "text/csv", "InformeProgreso.csv");
        }

        private IActionResult GenerarInformeXLSX(InformeProgreso informe, int progresoGeneral)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("InformeProgreso");
                worksheet.Cells["A1"].Value = "Proyecto";
                worksheet.Cells["B1"].Value = informe.Proyecto;
                worksheet.Cells["A2"].Value = "Supervisor";
                worksheet.Cells["B2"].Value = informe.Supervisor;
                worksheet.Cells["A3"].Value = "Fecha";
                worksheet.Cells["B3"].Value = informe.Fecha;
                worksheet.Cells["A4"].Value = "Estado";
                worksheet.Cells["B4"].Value = informe.Estado;
                worksheet.Cells["A5"].Value = "Progreso General";
                worksheet.Cells["B5"].Value = $"{progresoGeneral}%";
                worksheet.Cells["A7"].Value = "Tareas Completadas";
                worksheet.Cells["A8"].Value = "Nombre";
                worksheet.Cells["B8"].Value = "Fecha Inicio";
                worksheet.Cells["C8"].Value = "Fecha Fin";
                var row = 8;
                foreach (var tarea in informe.TareasCompletadas)
                {
                    worksheet.Cells[$"A{row}"].Value = tarea.Nombre;
                    worksheet.Cells[$"B{row}"].Value = tarea.FechaInicio;
                    worksheet.Cells[$"C{row}"].Value = tarea.FechaFin;
                    row++;
                }
                
                worksheet.Cells[$"A{row + 1}"].Value = "Próximas Tareas";
                worksheet.Cells[$"A{row + 2}"].Value = "Nombre";
                worksheet.Cells[$"B{row + 2}"].Value = "Fecha Inicio";
                worksheet.Cells[$"C{row + 2}"].Value = "Fecha Fin";
                row += 3;
                foreach (var tarea in informe.ProximasTareas)
                {
                    worksheet.Cells[$"A{row}"].Value = tarea.Nombre;
                    worksheet.Cells[$"B{row}"].Value = tarea.FechaInicio;
                    worksheet.Cells[$"C{row}"].Value = tarea.FechaFin;
                    row++; 
                }
                var xlsxBytes = package.GetAsByteArray();
                return File(xlsxBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InformeProgreso.xlsx");
            }
        }

        private string RenderizarInformeHTML(InformeProgreso informe, int progresoGeneral)
        {
            var html = $@"
            <html>
            <head><style>table, th, td {{border: 1px solid black; border-collapse: collapse;}}</style></head>
            <body>
                <h1>Informe de Progreso</h1>
                <p><strong>Proyecto:</strong> {informe.Proyecto}</p>
                <p><strong>Supervisor:</strong> {informe.Supervisor}</p>
                <p><strong>Fecha:</strong> {informe.Fecha}</p>
                <p><strong>Estado:</strong> {informe.Estado}</p>
                <p><strong>Progreso General:</strong> {progresoGeneral}%</p>
                <h2>Tareas Completadas</h2>
                <table>
                    <thead> 
                        <tr>
                            <th>Nombre</th>
                            <th>Fecha Inicio</th>
                            <th>Fecha Fin</th>
                        </tr>
                    </thead> 
                    <tbody>";
            foreach (var tarea in informe.TareasCompletadas)
            {
                html += $@"
                        <tr>
                            <td>{tarea.Nombre}</td>
                            <td>{tarea.FechaInicio}</td>
                            <td>{tarea.FechaFin}</td>
                        </tr>";
            }
            html += @"
                    </tbody>
                </table>
                <h2>Próximas Tareas</h2>
                <table>
                    <thead>
                        <tr>
                            <th>Nombre</th>
                            <th>Fecha Inicio</th>
                            <th>Fecha Fin</th>
                        </tr>
                    </thead>
                    <tbody>";
            foreach (var tarea in informe.ProximasTareas)
            {
                html += $@"
                            <tr>
                                <td>{tarea.Nombre}</td>
                                <td>{tarea.FechaInicio}</td>
                                <td>{tarea.FechaFin}</td>
                            </tr>";
            }
            html += @"
                        </tbody>
                    </table>
                    <h2>Incidencias</h2>
                    <ul>";
            foreach (var incidencia in informe.Incidencias)
            {
                html += $@"
                        <li>{incidencia}</li>";
            }
            html += @"
                    </ul>
                </body>
            </html>";
            return html;
        }
    }
}
