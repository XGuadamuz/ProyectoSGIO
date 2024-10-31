using System.ComponentModel.DataAnnotations;

namespace ProyectoSGIOCore.Models
{
    public class FacturaProveedor
    {
        [Key] // Definimos la clave primaria
        public int IdFactura { get; set; }

        [Required] // Asegura que el campo no sea nulo
        public int IdProveedor { get; set; } // Relación con Proveedor

        [Required]
        public DateTime FechaEmision { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser positivo.")]
        public decimal MontoTotal { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        public string Descripcion { get; set; }

        // Relación con la entidad Proveedor
        public Proveedor Proveedor { get; set; }
    }
}
