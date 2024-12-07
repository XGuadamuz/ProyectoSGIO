using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class FaseInicial
    {
        public int Id { get; set; }
        public int PlanInicialId { get; set; }

        [ForeignKey("PlanInicialId")]
        public PlanInicial PlanInicial { get; set; }

        public string Nombre { get; set; }
        public ICollection<TareaInicial> TareasIniciales { get; set; } = new List<TareaInicial>();
    }

}
