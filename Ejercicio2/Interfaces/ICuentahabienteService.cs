using Ejercicio2.Models;

namespace Ejercicio2.Interfaces
{
    public interface ICuentahabienteService
    {
        Task<Cuentahabiente> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Cuentahabiente cuentahabiente);
        Task<IEnumerable<Cuentahabiente>> GetAllAsync();
        Task<Cuentahabiente> CreateAsync(Cuentahabiente cuentahabiente);
        Task<bool> DeleteAsync(int id);
    }
}
