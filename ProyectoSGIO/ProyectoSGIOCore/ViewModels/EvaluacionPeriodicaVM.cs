using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.ViewModels
{
    public class EvaluacionPeriodicaVM
    {
        public Proyecto Proyecto { get; set; }
        public PlanInicial PlanInicial { get; set; }
        public List<dynamic> Desviaciones { get; set; }
        public List<ImpactoMedidaCorrectiva> ImpactoMedidas { get; set; }
    }
}
