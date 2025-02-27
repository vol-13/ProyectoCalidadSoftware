using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Models;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ProyectoCalidadSoftware.Services
{
    public class FileDatabaseService
    {
        private readonly EmpresaDbContext _context;
        private readonly string _filePath = @"C:\Users\v-jos\Desktop\U\2025\DataFlowManager\empleados.txt"; // Solo la ruta


        public FileDatabaseService(EmpresaDbContext context)
        {
            _context = context;
        }

        // Leer empleados desde el archivo .txt
        public List<Empleado> LeerEmpleadosDesdeArchivo()
        {
            var empleados = new List<Empleado>();

            if (File.Exists(_filePath))
            {
                var lineas = File.ReadAllLines(_filePath);

                foreach (var linea in lineas)
                {
                    var datos = linea.Split(',');

                    if (datos.Length == 4)
                    {
                        var empleado = new Empleado
                        {
                            Id = int.Parse(datos[0]),
                            Nombre = datos[1].Trim(),
                            Cargo = datos[2].Trim(),
                            DepartamentoId = int.Parse(datos[3].Trim())
                        };

                        empleados.Add(empleado);
                    }
                }
            }

            return empleados;
        }

        // Insertar empleados en la base de datos
        public void InsertarEmpleadosEnBaseDeDatos(List<Empleado> empleados)
        {
            foreach (var empleado in empleados)
            {
                // Verificar si el empleado ya existe
                var empleadoExistente = _context.Empleado.FirstOrDefault(e => e.Id == empleado.Id);

                if (empleadoExistente == null)
                {
                    // Si no existe, agregarlo a la base de datos
                    _context.Empleado.Add(empleado);
                }
                else
                {
                    // Si existe, actualizar los datos
                    empleadoExistente.Nombre = empleado.Nombre;
                    empleadoExistente.Cargo = empleado.Cargo;
                    empleadoExistente.DepartamentoId = empleado.DepartamentoId;
                }
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();
        }

        // Eliminar empleados que ya no están en el archivo
        public void EliminarEmpleadosNoEnArchivo(List<Empleado> empleadosDesdeArchivo)
        {
            var empleadosEnBaseDeDatos = _context.Empleado.ToList();

            // Eliminar empleados que no están en el archivo
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
