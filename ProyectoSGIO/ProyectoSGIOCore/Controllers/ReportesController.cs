using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using System.Security.Claims;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class ReportesController : Controller
    {
        private readonly AppDBContext _dbContext; 
        public ReportesController(AppDBContext dbContext)
        { 
            _dbContext = dbContext;
        }
        [HttpGet] 
        public IActionResult SubirReporte() 
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SubirReporte(IFormFile reporte)
        {
            if (reporte == null || reporte.Length == 0) 
            {
                ViewData["Mensaje"] = "No se seleccionó ningún archivo.";
                return View(); 
            } 

            // Validar el tamaño máximo
            if (reporte.Length > 300 * 1024 * 1024) 
            {
                ViewData["Mensaje"] = "El archivo excede el tamaño máximo permitido (300 MB).";
                return View();
            } 

            // Validar el formato del archivo
            string[] formatosPermitidos = { ".pdf", ".docx", ".xlsx" };
            string extension = Path.GetExtension(reporte.FileName).ToLower();
            
            if (!Array.Exists(formatosPermitidos, f => f == extension)) 
            {
                ViewData["Mensaje"] = $"El formato del archivo no está permitido: {extension}";
                return View();
            } 

            try 
            {
                // Guardar el archivo en el sistema de archivos
                string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "ReportesSubidos");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);
                
                string rutaReporte = Path.Combine(rutaCarpeta, reporte.FileName);
                using (var stream = new FileStream(rutaReporte, FileMode.Create))
                {
                    await reporte.CopyToAsync(stream);
                } 

                // Guardar la URL en la base de datos
                var nuevoReporte = new Reporte 
                {
                    Nombre = reporte.FileName,
                    FechaSubida = DateTime.Now,
                    UsuarioId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value) 
                };
                _dbContext.Reportes.Add(nuevoReporte);
                await _dbContext.SaveChangesAsync();
                ViewData["Mensaje"] = "Reporte subido exitosamente.";
            }
            catch (Exception) 
            {
                ViewData["Mensaje"] = "Hubo un problema de conexión y no se pudo subir el archivo. Por favor, inténtelo de nuevo.";
            }
            return View();
        } 
        [HttpGet] 
        public async Task<IActionResult> VerReportesRecientes()
        {
            var reportesRecientes = await _dbContext.Reportes
                .Include(r => r.Usuario) // Asegurarse de que se incluye la propiedad Gestor
                .OrderByDescending(r => r.FechaSubida) 
                .Take(10) // Mostrar los 10 reportes más recientes
                .ToListAsync(); 
            return View(reportesRecientes);
        } 
        [HttpPost] 
        public async Task<IActionResult> EliminarReporte(int id)
        {
            var reporte = await _dbContext.Reportes.FindAsync(id);
            if (reporte == null) 
            {
                ViewData["Mensaje"] = "Reporte no encontrado.";
                return RedirectToAction(nameof(VerReportesRecientes));
            }
            
            // Eliminar el archivo del sistema de archivos
            string rutaReporte = Path.Combine(Directory.GetCurrentDirectory(), "ReportesSubidos", reporte.Nombre);
            
            if (System.IO.File.Exists(rutaReporte))
            {
                System.IO.File.Delete(rutaReporte);
            } 
            
            // Eliminar la entrada de la base de datos
            _dbContext.Reportes.Remove(reporte);
            await _dbContext.SaveChangesAsync();
            ViewData["Mensaje"] = "Reporte eliminado exitosamente.";
            return RedirectToAction(nameof(VerReportesRecientes));
        }
        [HttpGet]
        public async Task<IActionResult> BuscarReporte(string criterio)
        {
            var reportes = await _dbContext.Reportes 
                .Include(r => r.Usuario) 
                .Where(r => r.Nombre.Contains(criterio) ||
                    r.Usuario.Nombre.Contains(criterio) ||
                    r.FechaSubida.ToString().Contains(criterio) ||
                    r.IdReporte.ToString().Contains(criterio)) 
                .ToListAsync(); return View(reportes);
        }
        
        [HttpGet]
        public async Task<IActionResult> DescargarReporte(int id)
        {
            var reporte = await _dbContext.Reportes.FindAsync(id);
            
            if (reporte == null)
            {
                ViewData["Mensaje"] = "Reporte no encontrado.";
                return RedirectToAction(nameof(VerReportesRecientes));
            }
            string rutaReporte = Path.Combine(Directory.GetCurrentDirectory(), "ReportesSubidos", reporte.Nombre);
            if (!System.IO.File.Exists(rutaReporte))
            {
                ViewData["Mensaje"] = "El archivo no existe en el sistema de archivos.";
                return RedirectToAction(nameof(VerReportesRecientes));
            }
            var memory = new MemoryStream();
            using (var stream = new FileStream(rutaReporte, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(rutaReporte), reporte.Nombre);
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string> 
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" } 
            };
        }
    }
}
