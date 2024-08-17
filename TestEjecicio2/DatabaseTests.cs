using Ejercicio2.Models;
using Microsoft.EntityFrameworkCore;

namespace TestEjecicio2
{
    public class DatabaseTests
    {
        private DbContextOptions<Ejercicio2DbContext> _options;

        public DatabaseTests()
        {
            _options = new DbContextOptionsBuilder<Ejercicio2DbContext>()
                .UseSqlServer("Server=DESKTOP-8J7LS49;Database=EmpresaDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;
        }

        [Fact]
        public void PuedeConectarDatabaseTablaDepartamentos()
        {
            using (var context = new Ejercicio2DbContext(_options))
            {
                var cuentahabiente = context.Cuentahabientes.ToList();
                Assert.NotNull(cuentahabiente);
                Assert.True(cuentahabiente.Count > 0, "No se encontraron departamentos en la base de datos.");
            }
        }

        [Fact]
        public void PuedeConectarDatabaseTablaAsociado()
        {
            using (var context = new Ejercicio2DbContext(_options))
            {
                var transaccione = context.Transacciones.ToList();
                Assert.NotNull(transaccione);
                Assert.True(transaccione.Count >= 0, "No se encontraron Asociado en la base de datos.");
            }
        }
    }
}