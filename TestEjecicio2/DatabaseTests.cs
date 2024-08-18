using Ejercicio2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TestEjecicio2
{
    public class DatabaseTests
    {
        private DbContextOptions<Ejercicio2DbContext> _options, _invalid;

        public DatabaseTests()
        {
            _options = new DbContextOptionsBuilder<Ejercicio2DbContext>()
                .UseSqlServer("Server=DESKTOP-8J7LS49;Database=BancoDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            _invalid = new DbContextOptionsBuilder<Ejercicio2DbContext>()
                .UseSqlServer("Server=INVALID_SERVER;Database=INVALID_DB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;
        }


        [Fact]
        public void Puede_Conectar_Database_TablaCuentahabientes()
        {
            // Arrange
            using var context = new Ejercicio2DbContext(_options);

            // Act
            var cuentahabientes = context.Cuentahabientes.ToList();

            // Assert
            Assert.NotNull(cuentahabientes);
            Assert.True(cuentahabientes.Count > 0, "No se encontraron Cuentahabientes en la base de datos.");
        }


        [Fact]
        public void Puede_Conectar_Database_TablaTransacciones()
        {
            // Arrange
            using var context = new Ejercicio2DbContext(_options);

            // Act
            List<Transaccione> transacciones;
            try
            {
                transacciones = context.Transacciones.ToList();
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Error al acceder a la tabla Transacciones: {ex.Message}");
                return;
            }

            // Assert
            Assert.NotNull(transacciones);
            Assert.True(transacciones.Count >= 0, "No se encontraron Transacciones en la base de datos.");
        }

        [Fact]
        public void Puede_Conectar_Database_TablaDenominaciones()
        {
            // Arrange
            using var context = new Ejercicio2DbContext(_options);

            // Act
            var denominaciones = context.Denominaciones.ToList();

            // Assert
            Assert.NotNull(denominaciones);
            Assert.True(denominaciones.Count >= 0, "No se encontraron Denominaciones en la base de datos.");
        }

        [Fact]
        public void Conectar_BaseDatos_Fallo()
        {
            // Arrange
            var exception = Record.Exception(() =>
            {
                using var context = new Ejercicio2DbContext(_invalid);
                // Act
                var denominaciones = context.Denominaciones.ToList();
            });

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<SqlException>(exception);
        }
    }
}