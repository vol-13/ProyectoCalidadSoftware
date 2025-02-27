using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCalidadSoftware.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Cargo { get; set; } = string.Empty;

        [ForeignKey("Departamento")]
        public int DepartamentoId { get; set; }

        public Departamento? Departamento { get; set; }
    }
}

