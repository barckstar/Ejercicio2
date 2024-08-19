namespace Ejercicio2.Interfaces
{
    public interface ICajeroAutomaticoService
    {
        Task<(bool, string)> RetirarDineroAsync(int cuentahabienteId, decimal cantidad);
    }
}
