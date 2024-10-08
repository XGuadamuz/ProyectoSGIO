using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministrativoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public AdministrativoController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarUsuarios()
        {
            // Obtener los usuarios de la base de datos, incluyendo los roles
            var usuarios = await _dbContext.Usuarios
                                           .Include(u => u.Rol)
                                           .ToListAsync();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {

            var roles = _dbContext.Roles.ToList();

            // Validación roles
            if (roles == null || !roles.Any())
            {
                ViewData["Mensaje"] = "No hay roles disponibles. Por favor, asegúrese de que los roles estén configurados.";
                return View();
            }

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(CrearUsuarioVM modelo)
        {
            // Cargar los roles
            var roles = await _dbContext.Roles.ToListAsync();
            ViewBag.Roles = roles;

            // Verifica si las contraseñas coinciden
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(modelo);
            }

            // Buscar el rol seleccionado
            var rolDefault = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == modelo.RolSeleccionado);

            if (rolDefault == null)
            {
                ViewData["Mensaje"] = "Debe seleccionar un Rol";
                return View(modelo);
            }

            // Crea el usuario
            Usuario usuario = new Usuario()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                IdRol = rolDefault.IdRol
            };

            // Guarda el usuario en la base de datos
            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            // Redirige si se creó correctamente
            if (usuario.IdUsuario != 0)
            {
                TempData["MensajeExito"] = "Usuario creado correctamente";
                return RedirectToAction("CrearUsuario", "Administrativo");
            }
            else
            {
                ViewData["Mensaje"] = "No se pudo crear el usuario";
            }
            return View(modelo);
        }
    }
}