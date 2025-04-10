using Microsoft.EntityFrameworkCore;
using ProyectoCalidadSoftware.Models;
//pruebas
namespace ProyectoCalidadSoftware.Data
{
    public class EmpresaDbContext : DbContext
    {
        public EmpresaDbContext(DbContextOptions<EmpresaDbContext> options) : base(options) { }

        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Departamento> Departamento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración opcional de la relación entre Empleado y Departamento
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Departamento)
                .WithMany(d => d.Empleados)
                .HasForeignKey(e => e.DepartamentoId);
        }
    }
}
