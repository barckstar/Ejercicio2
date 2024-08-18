using Ejercicio2.Models;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio2.Services
{
    public class CuentahabienteService
    {
        private readonly Ejercicio2DbContext _context;

        public CuentahabienteService(Ejercicio2DbContext context)
        {
            _context = context;
        }

        public async Task<List<Cuentahabiente>> GetAllAsync()
        {
            return await _context.Cuentahabientes.ToListAsync();
        }

        public async Task<Cuentahabiente?> GetByIdAsync(int id)
        {
            return await _context.Cuentahabientes.FindAsync(id);
        }

        public async Task<Cuentahabiente> CreateAsync(Cuentahabiente cuentahabiente)
        {
            _context.Cuentahabientes.Add(cuentahabiente);
            await _context.SaveChangesAsync();
            return cuentahabiente;
        }

        public async Task<bool> UpdateAsync(Cuentahabiente cuentahabiente)
        {
            _context.Cuentahabientes.Update(cuentahabiente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cuentahabiente = await _context.Cuentahabientes.FindAsync(id);
            if (cuentahabiente == null) return false;

            _context.Cuentahabientes.Remove(cuentahabiente);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
