using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiRestPostgreSql.Models;

public partial class Municipio
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del Municipio es obligatorio.")]
    public string? Nombre { get; set; }
    [Required(ErrorMessage = "El Departamento es obligatorio.")]
    public int? IdDepartamento { get; set; }

    public virtual Departamento? oDepartamento { get; set; }

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
