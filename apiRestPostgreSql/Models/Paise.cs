using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiRestPostgreSql.Models;

public partial class Paise
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del País es obligatorio.")]
    public string? Nombre { get; set; }

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
