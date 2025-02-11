using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiRestPostgreSql.Models;

public partial class Persona
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la Persona es obligatorio.")]
    public string? Nombre { get; set; }

    [Range(0, long.MaxValue, ErrorMessage = "El número de teléfono debe ser un número válido.")]
    public long? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "La dirección no puede exceder los 100 caracteres.")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "El municipio es obligatorio.")]
    public int? IdMunicipio { get; set; }

    
    public virtual Municipio? oMunicipio { get; set; }
}
