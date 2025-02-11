using Microsoft.AspNetCore.Mvc.Rendering;

namespace apiRestPostgreSql.Models.ViewModels
{
    public class MunicipioVM
    {
        public Municipio oMunicipio { get; set; }

        public List<SelectListItem> oListaDepartamento { get; set; }
    }
}
