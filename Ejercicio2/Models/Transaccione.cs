using System;
using System.Collections.Generic;

namespace Ejercicio2.Models;

public partial class Transaccione
{
    public int Id { get; set; }

    public int CuentahabienteId { get; set; }

    public string TipoTransaccion { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime FechaTransaccion { get; set; }

    public virtual Cuentahabiente Cuentahabiente { get; set; } = null!;
}
