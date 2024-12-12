using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class PlanInicial
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }

        [ForeignKey("ProyectoId")]
        public Proyecto Proyecto { get; set; }

        public ICollection<FaseInicial> FasesIniciales { get; set; } = new List<FaseInicial>();
    }

}
