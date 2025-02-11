using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiRestPostgreSql.Models;

public partial class Departamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El Nombre del Departamento es obligatorio.")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El País es obligatorio.")]
    public int? IdPais { get; set; }

    public virtual Paise? oPais { get; set; }

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
