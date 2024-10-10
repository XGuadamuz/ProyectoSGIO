namespace ProyectoSGIOCore.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public decimal HorasExtraAcumuladas { get; set; }
        public decimal UmbralHorasExtra { get; set; } = 40;

        public bool SuperaUmbralHorasExtra()
        {
            return HorasExtraAcumuladas > UmbralHorasExtra;
        }
    }
}
