using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoCalidadSoftware.Data;
using ProyectoCalidadSoftware.Models;
using ProyectoCalidadSoftware.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        private EmpresaDbContext _context;
        private FileDatabaseService _service;
        private Mock<ILogger<FileDatabaseService>> _loggerMock;
        private string _testFilePath = @"";

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EmpresaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new EmpresaDbContext(options);
            _loggerMock = new Mock<ILogger<FileDatabaseService>>();
            _service = new FileDatabaseService(_context, _loggerMock.Object);

            File.WriteAllLines(_testFilePath, new[]
            {
                "1,Juan Pérez,Gerente,2",
                "2,Ana López,Asistente,1"
            });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void ConexionBaseDeDatos()
        {
            var puedeConectarse = _context.Database.CanConnect();
            Assert.IsTrue(puedeConectarse, "La conexión a la base de datos falló.");
        }

        [TestMethod]
        public void LeerEmpleadosDesdeArchivo()
        {
            var empleados = _service.LeerEmpleadosDesdeArchivo();
            Assert.AreEqual(2, empleados.Count, "No se cargaron correctamente los empleados desde el archivo.");
            Assert.AreEqual("Juan Pérez", empleados[0].Nombre);
            Assert.AreEqual("Ana López", empleados[1].Nombre);
        }

        [TestMethod]
        public void InsertarEmpleadosBaseDatos()
        {
            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Carlos Ramírez", Cargo = "Analista", DepartamentoId = 1 },
                new Empleado { Nombre = "Laura Gómez", Cargo = "Desarrolladora", DepartamentoId = 2 }
            };

            _service.InsertarEmpleadosEnBaseDeDatos(empleados);
            var empleadosEnDb = _context.Empleado.ToList();

            Assert.AreEqual(2, empleadosEnDb.Count, "No se insertaron correctamente los empleados.");
            Assert.IsTrue(empleadosEnDb.Any(e => e.Nombre == "Carlos Ramírez"));
            Assert.IsTrue(empleadosEnDb.Any(e => e.Nombre == "Laura Gómez"));
        }

        [TestMethod]
        public void ActualizarEmpleados()
        {
            var empleadoExistente = new Empleado { Id = 1, Nombre = "Juan Pérez", Cargo = "Gerente", DepartamentoId = 2 };
            _context.Empleado.Add(empleadoExistente);
            _context.SaveChanges();

            var empleadosActualizados = new List<Empleado>
            {
                new Empleado { Id = 1, Nombre = "Juan Pérez", Cargo = "Director", DepartamentoId = 3 }
            };

            _service.InsertarEmpleadosEnBaseDeDatos(empleadosActualizados);
            var empleadoEnDb = _context.Empleado.FirstOrDefault(e => e.Id == 1);

            Assert.IsNotNull(empleadoEnDb, "El empleado no se encuentra en la base de datos.");
            Assert.AreEqual("Director", empleadoEnDb.Cargo, "El cargo del empleado no se actualizó correctamente.");
            Assert.AreEqual(3, empleadoEnDb.DepartamentoId, "El departamento del empleado no se actualizó correctamente.");
        }
    }
    
}