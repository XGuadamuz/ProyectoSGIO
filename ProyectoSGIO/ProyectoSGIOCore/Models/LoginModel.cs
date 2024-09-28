using System.ComponentModel.DataAnnotations;

namespace ProyectoSGIOCore.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; }
    }
}