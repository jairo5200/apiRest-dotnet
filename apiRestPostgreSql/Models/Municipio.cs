using System;
using System.Collections.Generic;

namespace apiRestPostgreSql.Models;

public partial class Municipio
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public int? IdDepartamento { get; set; }

    public virtual Departamento? oDepartamento { get; set; }

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
