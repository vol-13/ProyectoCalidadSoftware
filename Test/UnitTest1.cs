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
        private string _testFilePath = @"C:\Users\v-jos\Desktop\U\2025\DataFlowManager\Empleados-Test.txt";
        private string _exportFilePath = @"C:\Users\v-jos\Desktop\U\2025\DataFlowManager\empleadosDB-Test.txt";

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
                "1,Juan P�rez,Gerente,2",
                "2,Ana L�pez,Asistente,1",
                "3,Paula Mend�z,Asistente,3"
            });
        }

        [TestMethod]
        public void ConexionBaseDeDatos()
        {
            var puedeConectarse = _context.Database.CanConnect();
            Assert.IsTrue(puedeConectarse, "La conexi�n a la base de datos fall�.");
        }

        [TestMethod]
        public void LeerEmpleadosDesdeArchivo()
        {
            var empleados = _service.ReadEmployeesFile();
            Assert.AreEqual(2, empleados.Count, "No se cargaron correctamente los empleados desde el archivo.");
            Assert.AreEqual("Juan P�rez", empleados[0].Nombre);
            Assert.AreEqual("Ana L�pez", empleados[1].Nombre);
        }

        [TestMethod]
        public void InsertarEmpleadosBaseDatos()
        {
            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Carlos Ram�rez", Cargo = "Analista", DepartamentoId = 1 },
                new Empleado { Nombre = "Laura G�mez", Cargo = "Desarrolladora", DepartamentoId = 2 }
            };

            _service.InsertUpdateEmployees(empleados);
            var empleadosEnDb = _context.Empleado.ToList();

            Assert.AreEqual(2, empleadosEnDb.Count, "No se insertaron correctamente los empleados.");
            Assert.IsTrue(empleadosEnDb.Any(e => e.Nombre == "Carlos Ram�rez"));
            Assert.IsTrue(empleadosEnDb.Any(e => e.Nombre == "Laura G�mez"));
        }

        [TestMethod]
        public void EliminarEmpleadoConIdMasBajo()
        {
            // Insertamos empleados en la base de datos
            _context.Empleado.AddRange(new List<Empleado>
            {
                new Empleado { Nombre = "Carlos Ram�rez", Cargo = "Analista", DepartamentoId = 1 },
                new Empleado { Nombre = "Laura G�mez", Cargo = "Desarrolladora", DepartamentoId = 2 }
            });
            _context.SaveChanges();

            _service.DeleteEmployee();
            var empleadosEnDb = _context.Empleado.ToList();

            Assert.AreEqual(1, empleadosEnDb.Count, "No se elimin� correctamente el empleado con ID m�s bajo.");
            Assert.IsFalse(empleadosEnDb.Any(e => e.Id == 1), "El empleado con ID 1 no fue eliminado correctamente.");
        }

        [TestMethod]
        public void ExportEmployeeDB()
        {
            // Verificar si hay datos en la BD
            var empleadosEnDb = _context.Empleado.Count();
            Console.WriteLine($"Empleados en la BD antes de exportar: {empleadosEnDb}");

            Assert.IsTrue(empleadosEnDb > 0, "No hay empleados en la base de datos antes de la exportaci�n.");

            // Exportar datos
            _service.ReadDBData();

            // Esperar un poco por si el archivo tarda en crearse
            System.Threading.Thread.Sleep(100);

            // Verificar si el archivo existe
            bool archivoExiste = File.Exists(_exportFilePath);
            Console.WriteLine($"Archivo existe: {archivoExiste}");

            Assert.IsTrue(archivoExiste, $"El archivo de exportaci�n no se cre� en {_exportFilePath}.");
        }



    }
}
