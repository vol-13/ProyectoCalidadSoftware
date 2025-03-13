using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProyectoCalidadSoftware.Services
{
    public class FileDatabaseService
    {
        private readonly EmpresaDbContext _context;
        private readonly ILogger<FileDatabaseService> _logger;
        private readonly string _filePath = @"";

        public FileDatabaseService(EmpresaDbContext context, ILogger<FileDatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Leer empleados desde el archivo .txt
        public List<Empleado> LeerEmpleadosDesdeArchivo()
        {
            var empleados = new List<Empleado>();

            if (File.Exists(_filePath))
            {
                _logger.LogInformation($"El archivo {_filePath} existe. Iniciando lectura.");
                var lineas = File.ReadAllLines(_filePath);

                foreach (var linea in lineas)
                {
                    var datos = linea.Split(',');

                    if (datos.Length == 4)
                    {
                        var empleado = new Empleado
                        {
                            Nombre = datos[1].Trim(),
                            Cargo = datos[2].Trim(),
                            DepartamentoId = int.Parse(datos[3].Trim())
                        };

                        empleados.Add(empleado);
                    }
                }
            }
            else
            {
                _logger.LogWarning($"El archivo {_filePath} no se encuentra o no es accesible.");
            }

            return empleados;
        }

        // Insertar empleados en la base de datos
        public void InsertarEmpleadosEnBaseDeDatos(List<Empleado> empleados)
        {
            foreach (var empleado in empleados)
            {
                var empleadoExistente = _context.Empleado.FirstOrDefault(e => e.Id == empleado.Id);

                if (empleadoExistente == null)
                {
                    _logger.LogInformation($"Empleado {empleado.Nombre} no existe en la base de datos, se insertará.");
                    _context.Empleado.Add(empleado);
                }
                else
                {
                    _logger.LogInformation($"Empleado {empleado.Nombre} ya existe, se actualizará.");
                    empleadoExistente.Nombre = empleado.Nombre;
                    empleadoExistente.Cargo = empleado.Cargo;
                    empleadoExistente.DepartamentoId = empleado.DepartamentoId;
                }
            }

            _context.SaveChanges();
            _logger.LogInformation("Se guardaron los cambios en la base de datos.");
        }

        // Eliminar empleados que ya no están en el archivo
        public void EliminarEmpleadosNoEnArchivo(List<Empleado> empleadosDesdeArchivo)
        {
            var empleadosEnBaseDeDatos = _context.Empleado.ToList();

            foreach (var empleado in empleadosEnBaseDeDatos)
            {
                if (!empleadosDesdeArchivo.Any(e => e.Id == empleado.Id))
                {
                    _context.Empleado.Remove(empleado);
                }
            }

            _context.SaveChanges();
        }
    }
}
