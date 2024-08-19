using Ejercicio2.Models;
using Ejercicio2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestEjecicio2
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "S3cureP@ssw0rdF0rJWTAuth2024!SecretKey" },
                    { "Jwt:Issuer", "Ejercicio2" }
                })
                .Build();

            _authService = new AuthService(configuration);
        }

        [Fact]
        public void ValidateUser_Returns_Verdadero()
        {
            // Arrange
            var userLogin = new UserLogin { Username = "admin", Password = "admin" };

            // Act
            var result = _authService.ValidateUser(userLogin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateUser_Returns_Falso()
        {
            // Arrange
            var userLogin = new UserLogin { Username = "admin", Password = "wrongpassword" };

            // Act
            var result = _authService.ValidateUser(userLogin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GenerateToken_Returns_JWT_Token_Valido()
        {
            // Arrange
            var userLogin = new UserLogin { Username = "admin", Password = "admin" };

            // Act
            var token = _authService.GenerateToken(userLogin);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("S3cureP@ssw0rdF0rJWTAuth2024!SecretKey");

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

            var readToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            ClaimsPrincipal? principal = null;

            if (readToken != null)
            {
                var result = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                principal = result;
            }


            Assert.NotNull(principal);
            Assert.NotNull(principal?.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("admin", principal?.FindFirst(ClaimTypes.Name)?.Value);
        }

    }
}
