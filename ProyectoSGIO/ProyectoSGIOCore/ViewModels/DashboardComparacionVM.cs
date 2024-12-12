using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.ViewModels
{
    public class DashboardComparacionVM
    {
        public Proyecto Proyecto { get; set; }
        public PlanInicial PlanInicial { get; set; }
        public IEnumerable<dynamic> ProgresoActual { get; set; } // Puedes usar un tipo más específico si lo prefieres
    }
}
