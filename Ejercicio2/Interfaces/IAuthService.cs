using Ejercicio2.Models;

namespace Ejercicio2.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(UserLogin model);
        bool ValidateUser(UserLogin model);
    }
}
