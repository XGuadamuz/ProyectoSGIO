using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.ViewModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoSGIOCore.Services;
using Microsoft.Extensions.Hosting;

namespace ProyectoSGIOCore.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IUtilitariosModel _utilitariosModel;
        private readonly IHostEnvironment _hostEnvironment;

        public AccesoController(AppDBContext appDBContext, IUtilitariosModel utilitariosModel, IHostEnvironment hostEnvironment)
        {
            _dbContext = appDBContext;
            _utilitariosModel = utilitariosModel;
            _hostEnvironment = hostEnvironment;
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
                IdRol = rolDefault.IdRol,
                Activo = true
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
            string claveEncriptada = _utilitariosModel.Encrypt(modelo.Clave);
            // Buscar el usuario en la base de datos
            Usuario? usuario_encontrado = await _dbContext.Usuarios
                                        .Include(u => u.Rol)
                                        .Where(u => u.Correo == modelo.Correo && u.Clave == claveEncriptada)
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

            if (usuario_encontrado.Temporal)
            {
                ViewData["Mensaje"] = "Se requiere cambiar contraseña.";
                return RedirectToAction("CambiarContraseña", "Acceso");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RecuperarCuenta()
        {
            return View();

        }

        [HttpGet]
        public IActionResult CambiarContraseña()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> CambiarContraseña(UsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            Usuario? usuario_encontrado = _dbContext.Usuarios
                .FirstOrDefault(u => u.IdUsuario.ToString() == userId);

            // Verificar si el empleado existe 
            string contrasenna_nueva = _utilitariosModel.Encrypt(modelo.Clave);
            bool es_temporal = false;
            usuario_encontrado.Clave = contrasenna_nueva;
            usuario_encontrado.Temporal = es_temporal;

            var result = await _dbContext.SaveChangesAsync();

            if (result == 0)
            {
                ViewData["Mensaje"] = "Error al editar contraseña. Contacta al administrador para más información.";
                return View();
            }
            else {
                return RedirectToAction("IniciarSesion", "Acceso");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarCuenta(IniciarSesionVM modelo)
        {
            // Buscar el usuario en la base de datos
            Usuario? usuario_encontrado = await _dbContext.Usuarios
                                        .Include(u => u.Rol)
                                        .Where(u => u.Correo == modelo.Correo)
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

            string contrasenna_temporal = _utilitariosModel.GenerarCodigo();
            string contrasenna_temporal_nueva = _utilitariosModel.Encrypt(contrasenna_temporal);
            bool es_temporal = true;
            usuario_encontrado.Clave = contrasenna_temporal_nueva;
            usuario_encontrado.Temporal = es_temporal;

            var result = await _dbContext.SaveChangesAsync();

            if (result == 0)
            {
                ViewData["Mensaje"] = "Error al editar contraseña. Contacta al administrador para más información.";
                return View();
            }
            else
            {
                string ruta = Path.Combine(_hostEnvironment.ContentRootPath, "FormatoCorreo.html");
                string htmlBody = System.IO.File.ReadAllText(ruta);
                htmlBody = htmlBody.Replace("@nombre@", usuario_encontrado.Nombre);
                htmlBody = htmlBody.Replace("@apellido@", usuario_encontrado.Apellido);
                htmlBody = htmlBody.Replace("@correo@", usuario_encontrado.Correo);
                htmlBody = htmlBody.Replace("@contrasenna_temporal@", contrasenna_temporal);

                _utilitariosModel.EnviarCorreo(usuario_encontrado.Correo!, "Recuperar acceso en cuenta SGIO!", htmlBody);
                ViewData["Mensaje"] = "La nueva contraseña se envio a tu correo";
                return RedirectToAction("IniciarSesion", "Acceso");
            }
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