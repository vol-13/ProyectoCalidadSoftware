using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ProyectoCalidadSoftware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configura y corre la aplicación
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                    configuration
                        .WriteTo.Console() // Escribir en consola
                        .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)) // Escribir en archivo
                .ConfigureServices((context, services) =>
                {
                    // Registro del DbContext
                    services.AddDbContext<EmpresaDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("EmpresaDB")));

                    // Registrar Worker como IHostedService
                    services.AddHostedService<Worker>(); // Registrar Worker como un IHostedService
                });


    }
}
