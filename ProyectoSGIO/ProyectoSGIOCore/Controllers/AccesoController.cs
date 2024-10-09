using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.ViewModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProyectoSGIOCore.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _dbContext;
        public AccesoController(AppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioVM modelo)
        {
            // Verificar si el correo ya está en uso
            var usuarioExistente = await _dbContext.Usuarios
                                                   .FirstOrDefaultAsync(u => u.Correo == modelo.Correo);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError("Correo", "El correo electrónico ya está en uso.");
                return View(modelo);
            }

            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            var rolDefault = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == "Usuario");

            Usuario usuario = new Usuario()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                IdRol = rolDefault.IdRol
            };

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            if (usuario.IdUsuario != 0) return RedirectToAction("IniciarSesion", "Acceso");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(IniciarSesionVM modelo)
        {
            // Buscar el usuario en la base de datos
            Usuario? usuario_encontrado = await _dbContext.Usuarios
                                        .Include(u => u.Rol)
                                        .Where(u => u.Correo == modelo.Correo && u.Clave == modelo.Clave)
                                        .FirstOrDefaultAsync();

            // Si no se encuentra el usuario
            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            // Verificar si el usuario está inactivo/bloqueado
            if (!usuario_encontrado.Activo)
            {
                ViewData["Mensaje"] = "Tu cuenta ha sido bloqueada. Contacta al administrador para más información.";
                return View();
            }

            // Si el usuario está activo, proceder con el inicio de sesión
            List<Claim> claims = new List<Claim>(){
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.Rol.Nombre)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            // Autenticación exitosa
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IniciarSesion", "Acceso");
        }

        [HttpGet]
        public IActionResult VerPerfil()
        {
            // Obtener el Id del usuario autenticado desde los claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Datos del usuario desde la BD
            var usuario = _dbContext.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.IdUsuario.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
    }
}