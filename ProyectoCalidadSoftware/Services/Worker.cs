using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoCalidadSoftware
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando procesamiento...");

                    // Usar el scope para resolver DbContext y FileDatabaseService
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EmpresaDbContext>();
                        var fileDatabaseService = scope.ServiceProvider.GetRequiredService<FileDatabaseService>();

                        // Consultar empleados desde el DbContext
                        var empleados = context.Empleado.ToList();

                        if (empleados.Any())
                        {
                            _logger.LogInformation($"Se han encontrado {empleados.Count} empleados.");
                        }
                        else
                        {
                            _logger.LogWarning("No se encontraron empleados.");
                        }

                        // Leer empleados desde el archivo
                        var empleadosDesdeArchivo = fileDatabaseService.LeerEmpleadosDesdeArchivo();
                        if (empleadosDesdeArchivo.Any())
                        {
                            _logger.LogInformation($"Se han encontrado {empleadosDesdeArchivo.Count} empleados desde el archivo.");
                            // Insertar o actualizar empleados en la base de datos
                            fileDatabaseService.InsertarEmpleadosEnBaseDeDatos(empleadosDesdeArchivo);
                            _logger.LogInformation("Empleados insertados/actualizados en la base de datos.");
                        }
                        else
                        {
                            _logger.LogWarning("No se encontraron empleados en el archivo.");
                        }
                    }

                    // Espera antes de la siguiente ejecución
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al procesar los empleados: {ex.Message}");
                }
            }
        }
    }
}
