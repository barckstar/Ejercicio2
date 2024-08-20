using Ejercicio2.Controllers;
using Ejercicio2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace TestEjecicio2
{
    public class CajeroAutomaticoControllerTest
    {
        private readonly Mock<ICajeroAutomaticoService> _cajeroAutomaticoServiceMock;
        private readonly CajeroAutomaticoController _controller;

        public CajeroAutomaticoControllerTest()
        {
            _cajeroAutomaticoServiceMock = new Mock<ICajeroAutomaticoService>();
            _controller = new CajeroAutomaticoController(_cajeroAutomaticoServiceMock.Object);
        }

        [Fact]
        public async Task RetirarDinero_ReturnsOk_RetiroExitoso()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;
            var mensajeExito = "Retiro exitoso";
            _cajeroAutomaticoServiceMock
                .Setup(service => service.RetirarDineroAsync(cuentahabienteId, cantidad))
                .ReturnsAsync((true, mensajeExito));

            // Act
            var result = await _controller.RetirarDinero(cuentahabienteId, cantidad);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mensajeExito, okResult.Value!.GetType().GetProperty("Mensaje")?.GetValue(okResult.Value));
        }

        [Fact]
        public async Task RetirarDinero_ReturnsBadRequest_RetiroFalla()
        {
            // Arrange
            var cuentahabienteId = 1;
            var cantidad = 100m;
            var mensajeError = "Fondos insuficientes";
            _cajeroAutomaticoServiceMock
                .Setup(service => service.RetirarDineroAsync(cuentahabienteId, cantidad))
                .ReturnsAsync((false, mensajeError));

            // Act
            var result = await _controller.RetirarDinero(cuentahabienteId, cantidad);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(mensajeError, badRequestResult.Value!.GetType().GetProperty("Mensaje")?.GetValue(badRequestResult.Value));
        }


    }
}
