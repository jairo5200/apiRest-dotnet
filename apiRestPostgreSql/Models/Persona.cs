using System;
using System.Collections.Generic;

namespace apiRestPostgreSql.Models;

public partial class Persona
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public int? Telefono { get; set; }

    public string? Direccion { get; set; }

    public int? IdMunicipio { get; set; }

    public virtual Municipio? oMunicipio { get; set; }
}
