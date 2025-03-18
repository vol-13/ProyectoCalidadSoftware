using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProyectoCalidadSoftware.Services
{
    public class FileDatabaseService
    {
        private readonly EmpresaDbContext _context;
        private readonly ILogger<FileDatabaseService> _logger;
        private readonly string _filePath = @"C:\Users\v-jos\Desktop\U\2025\DataFlowManager\Empleados-2.txt";

        public FileDatabaseService(EmpresaDbContext context, ILogger<FileDatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Leer empleados desde el archivo .txt
        public List<Empleado> ReadEmployeesFile()
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
        // Insertar o actualizar empleados en la base de datos
        public void InsertUpdateEmployees(List<Empleado> empleados)
        {
            // Primero, obtenemos todos los empleados existentes de la base de datos
            var empleadosExistentes = _context.Empleado.ToList();

            foreach (var empleado in empleados)
            {
                // Usamos el Id para comparar si el empleado ya existe
                var empleadoExistente = empleadosExistentes.FirstOrDefault(e => e.Nombre == empleado.Nombre);

                if (empleadoExistente == null)
                {
                    // Si no existe, lo agregamos (la base de datos asignará el Id automáticamente)
                    _context.Empleado.Add(empleado);
                    _logger.LogInformation($"Empleado {empleado.Nombre} agregado.");
                }
                else
                {
                    // Si ya existe, actualizamos sus datos
                    empleadoExistente.Cargo = empleado.Cargo;
                    empleadoExistente.DepartamentoId = empleado.DepartamentoId;

                    // Marca al empleado como modificado
                    _context.Entry(empleadoExistente).State = EntityState.Modified;
                    _logger.LogInformation($"Empleado {empleadoExistente.Nombre} actualizado.");
                }
            }

            // Guardamos los cambios en la base de datos solo una vez
            _context.SaveChanges();
            _logger.LogInformation("Cambios guardados en la base de datos.");
        }



        public void DeleteEmployee()
        {
            // Obtenemos todos los empleados en la base de datos
            var empleadosEnBaseDeDatos = _context.Empleado.OrderBy(e => e.Id).ToList(); // Ordenamos por ID ascendente

            // Verificamos si hay empleados en la base de datos
            if (empleadosEnBaseDeDatos.Any())
            {
                // Tomamos el empleado con el ID más bajo (el primer empleado en la lista ordenada)
                var empleadoAEliminar = empleadosEnBaseDeDatos.First();

                // Lo eliminamos de la base de datos
                _context.Empleado.Remove(empleadoAEliminar);
                _context.SaveChanges(); // Guardamos los cambios en la base de datos

                _logger.LogInformation($"Empleado con ID {empleadoAEliminar.Id} y nombre {empleadoAEliminar.Nombre} ha sido eliminado.");
            }
            else
            {
                _logger.LogInformation("No hay empleados en la base de datos para eliminar.");
            }
        }



        public void ReadDBData()
        {
            // Leer todos los empleados de la base de datos
            var empleados = _context.Empleado.ToList();

            // Verificamos si hay empleados en la base de datos
            if (empleados.Any())
            {
                // Definimos el path del archivo
                string filePath = @"C:\Users\v-jos\Desktop\U\2025\DataFlowManager\empleadosDB.txt";

                // Creamos o sobrescribimos el archivo
                using (var writer = new StreamWriter(filePath))
                {
                    // Escribimos un encabezado (opcional)
                    writer.WriteLine("Id,Nombre,Cargo,DepartamentoId");

                    // Escribimos cada empleado en una línea del archivo
                    foreach (var empleado in empleados)
                    {
                        // Formateamos la información como CSV
                        writer.WriteLine($"{empleado.Id},{empleado.Nombre},{empleado.Cargo},{empleado.DepartamentoId}");
                    }
                }

                _logger.LogInformation($"Datos exportados correctamente a {filePath}");
            }
            else
            {
                _logger.LogWarning("No hay empleados en la base de datos para exportar.");
            }
        }



    }
}
