using Moq;
using Microsoft.AspNetCore.Mvc;
using Ejercicio2.Controllers;
using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Ejercicio2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestEjecicio2
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;
        private readonly AuthService _authService;

        public AuthControllerTest()
        {
            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                    { "Jwt:Key", "S3cureP@ssw0rdF0rJWTAuth2024!SecretKey" },
                    { "Jwt:Issuer", "Ejercicio2" }
               })
               .Build();

            _authService = new AuthService(configuration);
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public void GenerateToken_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var userLogin = new UserLogin { Username = "test", Password = "password" };
            _mockAuthService.Setup(service => service.ValidateUser(userLogin)).Returns(false);

            // Act
            var result = _controller.GenerateToken(userLogin);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GenerateToken_Returns_Valid_JWT_Token()
        {
            // Arrange
            var userLogin = new UserLogin { Username = "admin", Password = "admin" };

            // Act
            var token = _authService.GenerateToken(userLogin);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("S3cureP@ssw0rdF0rJWTAuth2024!SecretKey");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Ejercicio2",
                ValidAudience = "Ejercicio2",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var leerToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            ClaimsPrincipal? principal = null;
            if (leerToken != null)
            {
                var result = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                principal = result;
            }

            Assert.NotNull(principal);
            Assert.NotNull(principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("admin", principal.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}
