using System.ComponentModel.DataAnnotations;

namespace ProyectoCalidadSoftware.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Empleado>? Empleados { get; set; }
    }
}
