using ProyectoSGIOCore.Models;
using System.Collections.Generic;

namespace ProyectoSGIOCore.ViewModels
{
    public class CronogramaVM
    {
        public Proyecto Proyecto { get; set; }
        public List<Tarea> Tareas { get; set; }
        public List<Dependencia> Dependencias { get; set; }
    }
}

