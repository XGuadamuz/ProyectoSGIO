namespace ProyectoSGIOCore.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }


        // Clave foránea para el Rol
        public int IdRol { get; set; }
        public Rol Rol { get; set; } 

    }
}
