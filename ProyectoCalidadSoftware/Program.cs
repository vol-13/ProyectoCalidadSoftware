using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Services;
using Serilog;

namespace ProyectoCalidadSoftware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Crear y ejecutar la aplicación
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                    configuration
                        .WriteTo.Console()  // Mostrar en consola
                        .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)  // Escribir en archivo
                        .MinimumLevel.Debug())  // Mínimo nivel de log para depuración
                .ConfigureServices((context, services) =>
                {
                    // Registrar DbContext con la cadena de conexión
                    services.AddDbContext<EmpresaDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("EmpresaDB")));

                    // Registrar FileDatabaseService como Scoped
                    services.AddScoped<FileDatabaseService>();  

                    // Registrar Worker como IHostedService con ciclo de vida Singleton
                    services.AddSingleton<IHostedService, Worker>(); 
                });
    }
}
