using System;
using System.Collections.Generic;

namespace apiRestPostgreSql.Models;

public partial class Departamento
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public int? IdPais { get; set; }

    public virtual Paise? oPais { get; set; }

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
