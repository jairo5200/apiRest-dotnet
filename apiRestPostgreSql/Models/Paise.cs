using System;
using System.Collections.Generic;

namespace apiRestPostgreSql.Models;

public partial class Paise
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
