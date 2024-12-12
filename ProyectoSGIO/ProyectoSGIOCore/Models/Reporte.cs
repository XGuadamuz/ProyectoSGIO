using System.ComponentModel.DataAnnotations;

namespace ProyectoSGIOCore.Models
{
    public class Reporte
    {
        [Key] 
        public int IdReporte { get; set; }
        [Required]
        [StringLength(255)] 
        public string Nombre { get; set; } // Nombre del reporte
        public string Url { get; set; } 
        [Required] 
        public DateTime FechaSubida { get; set; } // Fecha de carga del reporte
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }


}
