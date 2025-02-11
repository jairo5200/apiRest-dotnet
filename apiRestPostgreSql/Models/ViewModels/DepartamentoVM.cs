using Microsoft.AspNetCore.Mvc.Rendering;

namespace apiRestPostgreSql.Models.ViewModels
{
    public class DepartamentoVM
    {
        public Departamento oDepartamento { get; set; }

        public List<SelectListItem> oListaPais { get; set; }
    }
}
