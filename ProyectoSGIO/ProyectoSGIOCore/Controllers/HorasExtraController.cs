using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using OfficeOpenXml;
using Azure.Core;

namespace ProyectoSGIOCore.Controllers
{
    public class HorasExtraController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IEmailService _emailService;

        public HorasExtraController(AppDBContext context)
        {
            _context = context;

        }

        // Escenario 1: Registro de horas extra
        [HttpPost]
        public async Task<IActionResult> RegistrarHorasExtra(int empleadoId, DateTime fecha, int cantidadHoras)
        {
            if (cantidadHoras <= 0) // Validación de horas
            {
                ModelState.AddModelError("", "La cantidad de horas debe ser mayor que cero.");
                return BadRequest(ModelState); // Devuelve un error 400 Bad Request
            }

            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado == null) // Validación de empleado existente
            {
                ModelState.AddModelError("", "El empleado no existe.");
                return NotFound(ModelState); // Devuelve un error 404 Not Found
            }

            var horaExtra = new HorasExtra
            {
                IdEmpleado = empleadoId,
                Fecha = fecha,
                CantidadHoras = cantidadHoras,
                Aprobada = false // Inicialmente no aprobada
            };

            try
            {
                _context.HorasExtra.Add(horaExtra);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al registrar horas extra: " + ex.Message);
                return StatusCode(500, ModelState); // Devuelve un error 500 Internal Server Error
            }

            return RedirectToAction("Index"); // Redirigir a la lista de horas extra
        }

        // Escenario 2: Verificación de acumulación de horas extra
        public async Task<IActionResult> VerHorasExtra(int empleadoId)
        {
            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado == null) // Validación de empleado existente
            {
                return NotFound(); // Devuelve un error 404 Not Found
            }

            var horasExtra = await _context.HorasExtra
                .Where(h => h.IdEmpleado == empleadoId)
                .ToListAsync();

            return View(horasExtra); // Retornar la vista con las horas extra
        }

        // Escenario 3: Ajuste de horas extra registradas incorrectamente
        [HttpPost]
        public async Task<IActionResult> EditarHorasExtra(int id, int cantidadHoras)
        {
            if (cantidadHoras <= 0) // Validación de horas
            {
                ModelState.AddModelError("", "La cantidad de horas debe ser mayor que cero.");
                return BadRequest(ModelState); // Devuelve un error 400 Bad Request
            }

            var horaExtra = await _context.HorasExtra.FindAsync(id);
            if (horaExtra == null) // Validación de existencia de horas extra
            {
                return NotFound(); // Devuelve un error 404 Not Found
            }

            try
            {
                horaExtra.CantidadHoras = cantidadHoras;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al editar horas extra: " + ex.Message);
                return StatusCode(500, ModelState); // Devuelve un error 500 Internal Server Error
            }

            return RedirectToAction("VerHorasExtra", new { empleadoId = horaExtra.IdEmpleado });
        }

        // Escenario 4: Aprobación de horas extra
        [HttpPost]
        public async Task<IActionResult> AprobarHorasExtra(int id)
        {
            var horaExtra = await _context.HorasExtra.FindAsync(id);
            if (horaExtra == null) // Validación de existencia de horas extra
            {
                return NotFound(); // Devuelve un error 404 Not Found
            }

            try
            {
                horaExtra.Aprobada = true;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al aprobar horas extra: " + ex.Message);
                return StatusCode(500, ModelState); // Devuelve un error 500 Internal Server Error
            }

            return RedirectToAction("VerHorasExtra", new { empleadoId = horaExtra.IdEmpleado });
        }

        // Escenario 5: Notificación de horas extra acumuladas
        public async Task<IActionResult> NotificarHorasExtra(dynamic request)
        {
            var empleadosConHorasExtra = await _context.HorasExtra
                .GroupBy(h => h.IdEmpleado)
                .Select(g => new
                {
                    IdEmpleado = g.Key,
                    TotalHoras = g.Sum(h => h.CantidadHoras)
                })
                .Where(e => e.TotalHoras > 40) // Umbral de horas extra
                .ToListAsync();

            var supervisorEmail = "supervisor@ejemplo.com"; // Obtener el correo del supervisor de tu base de datos
            var subject = "Registro de Horas Extras";
            var message = $"El empleado {request.EmployeeName} ha registrado {request.Hours} horas extras.";
            await _emailService.SendEmailAsync(supervisorEmail, subject, message);

            return Ok("Horas extras registradas y notificación enviada.");

            return View(empleadosConHorasExtra); // Retornar la vista con los empleados notificados
        }

        // Escenario 6: Generación de reporte de horas extra
        public async Task<IActionResult> GenerarReporte(string format)
        {
            var reporte = await _context.HorasExtra
                .Include(h => h.Empleado)
                .ToListAsync();

            if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
            {
                // Generar PDF
                using (var stream = new MemoryStream())
                {
                    var writer = new PdfWriter(stream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    // Agregar título
                    document.Add(new Paragraph("Reporte de Horas Extra").SetFontSize(20));

                    // Crear tabla
                    var tabla = new iText.Layout.Element.Table(3); 
                    tabla.AddHeaderCell("Empleado");
                    tabla.AddHeaderCell("Fecha");
                    tabla.AddHeaderCell("Cantidad Horas");

                    foreach (var horaExtra in reporte)
                    {
                        tabla.AddCell(horaExtra.Empleado.Nombre); 
                        tabla.AddCell(horaExtra.Fecha.ToString("d"));
                        tabla.AddCell(horaExtra.CantidadHoras.ToString());
                    }

                    document.Add(tabla);
                    document.Close();

                    // Retornar el PDF
                    var nombreArchivo = "Reporte_Horas_Extra.pdf";
                    return File(stream.ToArray(), "application/pdf", nombreArchivo);
                }
            }
            else if (string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase))
            {
                // Generar Excel
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Horas Extra");

                    // Agregar encabezados
                    worksheet.Cells[1, 1].Value = "Empleado";
                    worksheet.Cells[1, 2].Value = "Fecha";
                    worksheet.Cells[1, 3].Value = "Cantidad Horas";

                    int row = 2; // Comenzar en la segunda fila

                    foreach (var horaExtra in reporte)
                    {
                        worksheet.Cells[row, 1].Value = horaExtra.Empleado.Nombre; // Suponiendo que tienes una propiedad Nombre
                        worksheet.Cells[row, 2].Value = horaExtra.Fecha.ToString("d");
                        worksheet.Cells[row, 3].Value = horaExtra.CantidadHoras;
                        row++;
                    }

                    // Configurar el contenido para ser descargado
                    var nombreArchivo = "Reporte_Horas_Extra.xlsx";
                    var fileStream = new MemoryStream();
                    package.SaveAs(fileStream);
                    fileStream.Position = 0; // Restablecer el flujo

                    return File(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }

            // Si no se especifica un formato válido, retornar un error
            return BadRequest("Formato no válido. Por favor, use 'pdf' o 'excel'.");
        }

        // Escenario 7: Compensación de horas extra con tiempo libre
        [HttpPost]
        public async Task<IActionResult> CompensarConTiempoLibre(int id, int diasLibres)
        {
            if (diasLibres <= 0) // Validación de días libres
            {
                ModelState.AddModelError("", "La cantidad de días libres debe ser mayor que cero.");
                return BadRequest(ModelState); // Devuelve un error 400 Bad Request
            }

            var horaExtra = await _context.HorasExtra.FindAsync(id);
            if (horaExtra == null) // Validación de existencia de horas extra
            {
                return NotFound(); // Devuelve un error 404 Not Found
            }

            try
            {
                // Ajustar el saldo de tiempo libre del empleado aquí
                await AjustarTiempoLibre(horaExtra.IdEmpleado, diasLibres);

                // Eliminar o marcar las horas extra como compensadas
                _context.HorasExtra.Remove(horaExtra);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al compensar horas extra: " + ex.Message);
                return StatusCode(500, ModelState); // Devuelve un error 500 Internal Server Error
            }

            return RedirectToAction("Index");
        }

        private async Task AjustarTiempoLibre(int empleadoId, int diasLibres)
        {
            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado != null)
            {
                empleado.TiempoLibre += diasLibres; // Aumentar tiempo libre
                await _context.SaveChangesAsync();
            }
        }
    }
}

