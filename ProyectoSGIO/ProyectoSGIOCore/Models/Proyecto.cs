using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Relación con Fases
        public ICollection<Fase> Fases { get; set; } = new List<Fase>();

        // Relación con Cliente
        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
    }
}