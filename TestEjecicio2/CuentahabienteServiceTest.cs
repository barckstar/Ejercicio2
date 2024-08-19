using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Ejercicio2.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestEjecicio2
{
    public class CuentahabienteServiceTests
    {
        private DbContextOptions<Ejercicio2DbContext> GetInMemoryDbOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<Ejercicio2DbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }
        [Fact]
        public async Task GetAllAsync_Returns_All_Cuentahabientes()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetAllAsyncTestDb");

            var cuentahabientes = new[]
            {
                new Cuentahabiente { Id = 1, NombreCompleto = "Test1", Saldo = 100 },
                new Cuentahabiente { Id = 2, NombreCompleto = "Test2", Saldo = 200 }
            };

            using (var context = new Ejercicio2DbContext(options))
            {
                context.Cuentahabientes.AddRange(cuentahabientes);
                await context.SaveChangesAsync();
            }

            using var contextForGetAll = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(contextForGetAll);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == 1 && c.NombreCompleto == "Test1" && c.Saldo == 100);
            Assert.Contains(result, c => c.Id == 2 && c.NombreCompleto == "Test2" && c.Saldo == 200);
        }

        [Fact]
        public async Task GetAllAsync_Returns_Empty_When_No_Cuentahabientes()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetAllAsyncEmptyTestDb");

            using var context = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Cuentahabiente_When_Found()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetByIdAsyncTestDb");

            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test", Saldo = 100 };

            using (var context = new Ejercicio2DbContext(options))
            {
                context.Cuentahabientes.Add(cuentahabiente);
                await context.SaveChangesAsync();
            }

            using var contextForGetById = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(contextForGetById);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cuentahabiente.Id, result.Id);
            Assert.Equal(cuentahabiente.NombreCompleto, result.NombreCompleto);
            Assert.Equal(cuentahabiente.Saldo, result.Saldo);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Null_When_Not_Found()
        {
            // Arrange
            var options = GetInMemoryDbOptions("GetByIdAsyncNotFoundTestDb");

            using var context = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(context);

            // Act
            var result = await service.GetByIdAsync(999); // ID que no existe

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_Crear_Nuevo_Cuentahabiente()
        {
            // Arrange
            var options = GetInMemoryDbOptions("CreateAsyncTestDb");

            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test", Saldo = 100 };

            using var context = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(context);

            // Act
            var result = await service.CreateAsync(cuentahabiente);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cuentahabiente.Id, result.Id);
            Assert.Equal(cuentahabiente.NombreCompleto, result.NombreCompleto);
            Assert.Equal(cuentahabiente.Saldo, result.Saldo);

            var createdCuentahabiente = await context.Cuentahabientes.FindAsync(cuentahabiente.Id);
            Assert.NotNull(createdCuentahabiente);
        }

        [Fact]
        public async Task UpdateAsync_Actulizar_Cuentahabiente()
        {
            // Arrange
            var options = GetInMemoryDbOptions("UpdateAsyncTestDb");

            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test", Saldo = 100 };

            using (var context = new Ejercicio2DbContext(options))
            {
                context.Cuentahabientes.Add(cuentahabiente);
                await context.SaveChangesAsync();
            }

            using var contextForUpdate = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(contextForUpdate);

            cuentahabiente.NombreCompleto = "Updated Test";
            cuentahabiente.Saldo = 200m;

            // Act
            var result = await service.UpdateAsync(cuentahabiente);

            // Assert
            Assert.True(result);

            var updatedCuentahabiente = await contextForUpdate.Cuentahabientes.FindAsync(cuentahabiente.Id);
            Assert.NotNull(updatedCuentahabiente);
            Assert.Equal("Updated Test", updatedCuentahabiente.NombreCompleto);
            Assert.Equal(200m, updatedCuentahabiente.Saldo);
        }

        [Fact]
        public async Task DeleteAsync_Eliminar_Cuentahabiente()
        {
            // Arrange
            var options = GetInMemoryDbOptions("DeleteAsyncTestDb");

            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test", Saldo = 100 };

            using (var context = new Ejercicio2DbContext(options))
            {
                context.Cuentahabientes.Add(cuentahabiente);
                await context.SaveChangesAsync();
            }

            using var contextForDelete = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(contextForDelete);

            // Act
            var result = await service.DeleteAsync(cuentahabiente.Id);

            // Assert
            Assert.True(result);

            var deletedCuentahabiente = await contextForDelete.Cuentahabientes.FindAsync(cuentahabiente.Id);
            Assert.Null(deletedCuentahabiente);
        }

        [Fact]
        public async Task DeleteAsync_Returns_False_Si_Cuentahabiente_No_Encontrado()
        {
            // Arrange
            var options = GetInMemoryDbOptions("DeleteAsyncNotFoundTestDb");

            using var context = new Ejercicio2DbContext(options);
            var service = new CuentahabienteService(context);

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
