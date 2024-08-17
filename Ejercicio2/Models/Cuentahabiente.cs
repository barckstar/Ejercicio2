using System;
using System.Collections.Generic;

namespace Ejercicio2.Models;

public partial class Cuentahabiente
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public decimal Saldo { get; set; }

    public DateTime FechaAlta { get; set; }

    public virtual ICollection<Transaccione> Transacciones { get; set; } = new List<Transaccione>();
}
