using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace apiRestPostgreSql.Controllers
{
    public class PersonaController : Controller
    {
        private readonly PruebacoinkdbContext _DBContext;
        public PersonaController(PruebacoinkdbContext context) {
            _DBContext = context;
        }

        public IActionResult Index()
        {
            List<Persona> personas = _DBContext.Personas.Include(e => e.oMunicipio).ToList();
            return View(personas);
        }


        [HttpGet]
        public IActionResult PersonaDetalle(int idPersona)
        {
            PersonaVM oPersonaVM = new PersonaVM()
            {
                oPersona = new Persona(),
                oListaMunicipios = _DBContext.Municipios.Select(municipio => new SelectListItem()
                {
                    Text = municipio.Nombre,
                    Value = municipio.Id.ToString()
                }).ToList()
            };

            if (idPersona != 0)
            {
                oPersonaVM.oPersona = _DBContext.Personas.Find(idPersona);
            }
            return View(oPersonaVM);
        }

        [HttpPost]
        public IActionResult PersonaDetalle(PersonaVM oPersonaVM)
        {
            if (oPersonaVM.oPersona.Id == 0)
            {
                _DBContext.Personas.Add(oPersonaVM.oPersona);
            }
            else
            {
                _DBContext.Personas.Update(oPersonaVM.oPersona);
            }

            _DBContext.SaveChanges();

            return RedirectToAction("Index", "Persona");
        }

        [HttpGet]
        public IActionResult EliminarPersona(int idPersona)
        {
            Persona oPersona = _DBContext.Personas.Include(m => m.oMunicipio).Where(e => e.Id == idPersona).FirstOrDefault();
            return View(oPersona);

        }
        [HttpPost]
        public IActionResult EliminarPersona(Persona oPersona)
        {

            _DBContext.Personas.Remove(oPersona);
            _DBContext.SaveChanges();
            return RedirectToAction("Index", "Persona");

        }
    }
}
