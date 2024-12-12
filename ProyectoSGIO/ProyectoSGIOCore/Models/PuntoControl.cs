using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class PuntoControl
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ICollection<FaseControl> FasesControl { get; set; } = new List<FaseControl>();

        [ForeignKey("ProyectoId")]
        public Proyecto Proyecto { get; set; }
    }

    public class FaseControl
    {
        public int Id { get; set; }
        public int PuntoControlId { get; set; }
        public string Nombre { get; set; }
        public ICollection<TareaControl> TareasControl { get; set; } = new List<TareaControl>();

        [ForeignKey("PuntoControlId")]
        public PuntoControl PuntoControl { get; set; }
    }

    public class TareaControl
    {
        public int Id { get; set; }
        public int FaseControlId { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Completada { get; set; }

        [ForeignKey("FaseControlId")]
        public FaseControl FaseControl { get; set; }
    }

}
