using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Ejercicio2.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestEjecicio2
{
    public class CajeroAutomaticoServiceTest
    {
        private DbContextOptions<Ejercicio2DbContext> GetInMemoryDbOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<Ejercicio2DbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        [Fact]
        public async Task Retirar_Dinero_Retiro_Exitoso()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(new Cuentahabiente { Id = cuentahabienteId, Saldo = 200m });

            mockCuentahabienteService.Setup(s => s.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(true);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase1"));
            Seed(context); // Inicializar datos de prueba
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.True(result.Item1, "El retiro debería ser exitoso.");
            Assert.Contains("100", result.Item2);
            Assert.Single(context.Transacciones);
            Assert.Equal(cantidad, context.Transacciones.First().Monto);
            Assert.Equal("Retiro", context.Transacciones.First().TipoTransaccion);
        }

        [Fact]
        public async Task Retirar_Dinero_Saldo_Insuficiente()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 300m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(new Cuentahabiente { Id = cuentahabienteId, Saldo = 200m });

            mockCuentahabienteService.Setup(s => s.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(true);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase2"));
            Seed(context); // Inicializar datos de prueba
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.False(result.Item1, "El retiro no debería ser permitido con saldo insuficiente.");
            Assert.Equal("Saldo insuficiente.", result.Item2);
            Assert.Empty(context.Transacciones);
        }

        [Fact]
        public async Task Retirar_Dinero_Cuentahabiente_No_Encontrado()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            _ = mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync((Cuentahabiente)null);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase3"));
            Seed(context); // Inicializar datos de prueba
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.False(result.Item1, "El retiro no debería ser permitido si el cuentahabiente no existe.");
            Assert.Equal("Cuentahabiente no encontrado.", result.Item2);
            Assert.Empty(context.Transacciones);
        }

        private void Seed(Ejercicio2DbContext context)
        {
            if (context.Denominaciones.Any()) return; // Ya se han inicializado

            context.Denominaciones.AddRange(new[]
            {
                new Denominacione { Nombre = "Billete de 100", Valor = 100 },
                new Denominacione { Nombre = "Billete de 50", Valor = 50 },
                new Denominacione { Nombre = "Billete de 20", Valor = 20 },
                new Denominacione { Nombre = "Moneda de 1", Valor = 1 }
            });

            context.SaveChanges();
        }
    }
}
