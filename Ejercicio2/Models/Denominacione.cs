using System;
using System.Collections.Generic;

namespace Ejercicio2.Models;

public partial class Denominacione
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Valor { get; set; }
}
