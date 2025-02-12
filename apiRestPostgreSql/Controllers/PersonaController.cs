using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            List<Persona> personas = _DBContext.Personas
                .FromSqlRaw("SELECT * FROM obtener_personas()")
                .ToList();

            foreach (var persona in personas)
            {
                if (persona.IdMunicipio != null)
                {
                    persona.oMunicipio = _DBContext.Municipios.Find(persona.IdMunicipio);
                }
            }

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
                // Llamar a la función almacenada para obtener la persona
                var persona = _DBContext.Personas
                    .FromSqlRaw("SELECT * FROM obtener_persona_detalle({0})", idPersona)
                    .AsEnumerable() // Convierte a IEnumerable para poder acceder a los resultados
                    .FirstOrDefault();

                if (persona != null)
                {
                    oPersonaVM.oPersona = persona;
                }
            }

            return View(oPersonaVM);
        }

        [HttpPost]
        public IActionResult PersonaDetalle(PersonaVM oPersonaVM)
        {
            ModelState.Remove("oListaMunicipios");
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, vuelve a mostrar la vista con los errores
                oPersonaVM.oListaMunicipios = _DBContext.Municipios.Select(municipio => new SelectListItem
                {
                    Text = municipio.Nombre,
                    Value = municipio.Id.ToString()
                }).ToList();
                return View(oPersonaVM);
            }
            else
            {
                // Llamar al procedimiento almacenado para insertar o actualizar la persona
                _DBContext.Database.ExecuteSqlRaw("CALL guardar_persona({0}, {1}, {2}, {3}, {4})",
                    oPersonaVM.oPersona.Id,
                    oPersonaVM.oPersona.Nombre,
                    oPersonaVM.oPersona.Telefono,
                    oPersonaVM.oPersona.Direccion,
                    oPersonaVM.oPersona.IdMunicipio);

                return RedirectToAction("Index", "Persona");
            }

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
            // Llamar al procedimiento almacenado para eliminar la persona
            _DBContext.Database.ExecuteSqlRaw("CALL eliminar_persona({0})", oPersona.Id);

            return RedirectToAction("Index", "Persona");
        }
    }
}
