using Microsoft.AspNetCore.Mvc.Rendering;

namespace apiRestPostgreSql.Models.ViewModels
{
    public class PersonaVM
    {
        public Persona oPersona { get; set; }

        public List<SelectListItem> oListaMunicipios { get; set; }
    }
}
