using System.ComponentModel.DataAnnotations;

namespace ProyectoSGIOCore.Models
{
    public class Archivo
    {
        [Key]
        public int IdArchivo { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; } // Nombre del archivo
        public string Url { get; set; }

        [Required]
        public DateTime FechaSubida { get; set; } // Fecha de carga del archivo
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}   
