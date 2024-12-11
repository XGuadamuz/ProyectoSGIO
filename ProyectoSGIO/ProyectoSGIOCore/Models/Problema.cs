using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class Problema
    {
        public int ID { get; set; }
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }

        public string Descripcion { get; set; }
        public int estado { get; set; }

        public int categoria { get; set; }

        public int prioridad { get; set; }
        // Relación con Usuario
        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        public DateTime Fecha { get; set; }
    }
}

