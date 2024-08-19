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
        public async Task RetirarDineroAsync_Retiro_Exitoso()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(new Cuentahabiente { Id = cuentahabienteId, Saldo = 200 });

            mockCuentahabienteService.Setup(s => s.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(true);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase1"));
            Seed(context);
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
        public async Task RetirarDineroAsync_Saldo_Insuficiente()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 300m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(new Cuentahabiente { Id = cuentahabienteId, Saldo = 200 });

            mockCuentahabienteService.Setup(s => s.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(true);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase2"));
            Seed(context);
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.False(result.Item1, "El retiro no debería ser permitido con saldo insuficiente.");
            Assert.Equal("Saldo insuficiente.", result.Item2);
            Assert.Empty(context.Transacciones);
        }

        [Fact]
        public async Task RetirarDineroAsync_Cuentahabiente_No_Encontrado()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(null as Cuentahabiente);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase3"));
            Seed(context);
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.False(result.Item1, "El retiro no debería ser permitido si el cuentahabiente no existe.");
            Assert.Equal("Cuentahabiente no encontrado.", result.Item2);
            Assert.Empty(context.Transacciones);
        }

        [Fact]
        public async Task RetirarDineroAsync_Error_Al_Actualizar_Saldo()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;

            var mockCuentahabienteService = new Mock<ICuentahabienteService>();
            mockCuentahabienteService.Setup(s => s.GetByIdAsync(cuentahabienteId))
                .ReturnsAsync(new Cuentahabiente { Id = cuentahabienteId, Saldo = 200 });

            mockCuentahabienteService.Setup(s => s.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(false);

            using var context = new Ejercicio2DbContext(GetInMemoryDbOptions("TestDatabase4"));
            Seed(context);
            var service = new CajeroAutomaticoService(mockCuentahabienteService.Object, context);

            // Act
            var result = await service.RetirarDineroAsync(cuentahabienteId, cantidad);

            // Assert
            Assert.False(result.Item1, "El retiro debería fallar si hay un error al actualizar el saldo.");
            Assert.Equal("Error al actualizar el saldo.", result.Item2);
            Assert.Empty(context.Transacciones);
        }


        private void Seed(Ejercicio2DbContext context)
        {
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
