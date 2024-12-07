using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class ImpactoMedidaCorrectiva
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string Fase { get; set; }
        public string Medida { get; set; }
        public DateTime FechaImplementacion { get; set; }
        public string Impacto { get; set; }

        [ForeignKey("ProyectoId")]
        public Proyecto Proyecto { get; set; }
    }

}
