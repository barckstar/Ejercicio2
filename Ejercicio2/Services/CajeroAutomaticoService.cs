using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio2.Services
{
    public class CajeroAutomaticoService : ICajeroAutomaticoService
    {
        private readonly ICuentahabienteService _cuentahabienteService;
        private readonly Ejercicio2DbContext _context;

        public CajeroAutomaticoService(ICuentahabienteService cuentahabienteService, Ejercicio2DbContext context)
        {
            _cuentahabienteService = cuentahabienteService;
            _context = context;
        }

        private async Task<Dictionary<string, int>> CalcularDenominacionesAsync(decimal cantidad)
        {
            var denominaciones = await _context.Denominaciones.OrderByDescending(d => d.Valor).ToListAsync();
            var resultado = new Dictionary<string, int>();

            foreach (var denominacion in denominaciones)
            {
                int cantidadBilletesOMonedas = (int)(cantidad / denominacion.Valor);
                if (cantidadBilletesOMonedas > 0)
                {
                    resultado[denominacion.Nombre] = cantidadBilletesOMonedas;
                    cantidad -= cantidadBilletesOMonedas * denominacion.Valor;
                }
            }

            return resultado;
        }

        public async Task<(bool, string)> RetirarDineroAsync(int cuentahabienteId, decimal cantidad)
        {
            var cuentahabiente = await _cuentahabienteService.GetByIdAsync(cuentahabienteId);
            if (cuentahabiente == null)
            {
                return (false, "Cuentahabiente no encontrado.");
            };

            if (cuentahabiente.Saldo < cantidad)
            {
                return (false, "Saldo insuficiente.");
            };

            cuentahabiente.Saldo -= cantidad;
            var result = await _cuentahabienteService.UpdateAsync(cuentahabiente);
            if (!result)
            {
                return (false, "Error al actualizar el saldo.");
            };

            _context.Transacciones.Add(new Transaccione
            {
                CuentahabienteId = cuentahabienteId,
                Monto = cantidad,
                FechaTransaccion = DateTime.UtcNow,
                TipoTransaccion = "Retiro"
            });

            await _context.SaveChangesAsync();

            var denominaciones = await CalcularDenominacionesAsync(cantidad);
            return (true, string.Join(", ", denominaciones.Select(d => $"{d.Key}: {d.Value}")));
        }
    }
}
