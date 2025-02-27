using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoCalidadSoftware.Data;
using Microsoft.Extensions.DependencyInjection; // Necesario para IServiceScopeFactory
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoCalidadSoftware.Services
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

                    // Usar el scope para resolver DbContext
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EmpresaDbContext>();

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
