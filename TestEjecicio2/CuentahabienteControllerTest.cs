using Ejercicio2.Controllers;
using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestEjecicio2
{
    public class CuentahabienteControllerTest
    {
        private readonly Mock<ICuentahabienteService> _mockService;
        private readonly CuentahabienteController _controller;

        public CuentahabienteControllerTest()
        {
            _mockService = new Mock<ICuentahabienteService>();
            _controller = new CuentahabienteController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCuentahabientes()
        {
            // Arrange
            var cuentahabientes = new List<Cuentahabiente> { new Cuentahabiente { Id = 1, NombreCompleto = "Test" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(cuentahabientes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Cuentahabiente>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithCuentahabiente()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(cuentahabiente);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Cuentahabiente>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCuentahabienteNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Cuentahabiente)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WithNewCuentahabiente()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test" };
            _mockService.Setup(s => s.CreateAsync(cuentahabiente)).ReturnsAsync(cuentahabiente);

            // Act
            var result = await _controller.Create(cuentahabiente);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<Cuentahabiente>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Updated" };
            _mockService.Setup(s => s.UpdateAsync(cuentahabiente)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(1, cuentahabiente);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCuentahabienteIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("{ mensaje = Los datos del cuentahabiente son necesarios. }", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test" };

            // Act
            var result = await _controller.Update(2, cuentahabiente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("{ mensaje = ID de cuentahabiente no coincide. }", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenUpdateFails()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "John Doe" };

            // Configurar el mock para que UpdateAsync retorne false
            _mockService
                .Setup(service => service.UpdateAsync(It.IsAny<Cuentahabiente>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(1, cuentahabiente);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenCreateFails()
        {
            // Arrange
            var cuentahabiente = new Cuentahabiente { Id = 1, NombreCompleto = "Test" };

            // Configurar el mock para que CreateAsync retorne null
            _mockService
                .Setup(service => service.CreateAsync(cuentahabiente))
                .ReturnsAsync((Cuentahabiente)null);

            // Act
            var result = await _controller.Create(cuentahabiente);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
        {
            // Arrange
            var id = 1;

            // Configurar el mock para que DeleteAsync retorne false
            _mockService
                .Setup(service => service.DeleteAsync(id))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
