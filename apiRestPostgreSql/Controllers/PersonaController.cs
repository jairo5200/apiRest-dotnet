using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

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
            try
            {
                // Obtenemos la lista de personas
                List<Persona> personas = _DBContext.Personas
                    .FromSqlRaw("SELECT * FROM obtener_personas()")
                    .ToList();

                // Iteramos la lista de personas
                foreach (var persona in personas)
                {
                    // Validamos que la persona tenga municipio
                    if (persona.IdMunicipio != null)
                    {
                        // Asignamos el municipio a la persona
                        persona.oMunicipio = _DBContext.Municipios.Find(persona.IdMunicipio);
                    }
                }

                return View(personas);
            }
            catch (PostgresException ex)
            {
                // Capturamos la excepción específica de PostgreSQL
                string errorMessage = $"Error: {ex.MessageText} (Código: {ex.SqlState})";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la pagina Error para mostrar el Error
            }
        }


        [HttpGet]
        public IActionResult PersonaDetalle(int idPersona)
        {
            try
            {
                // Generamos una persona nueva y el listado de municipios
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
                    // Validamos que la persona no sea null
                    if (persona != null)
                    {
                        // Asignamos la persona al ViewModel
                        oPersonaVM.oPersona = persona;
                    }
                }

                return View(oPersonaVM);
            }
            catch (PostgresException ex)
            {
                // Capturamos la excepción específica de PostgreSQL
                string errorMessage = $"Error: {ex.Message} (Código: {ex.SqlState})";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la pagina Error para mostrar el Error
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción
                string errorMessage = $"Error: {ex.Message}";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la pagina Error para mostrar el Error
            }

        }

        [HttpPost]
        public IActionResult PersonaDetalle(PersonaVM oPersonaVM)
        {
            // Removemos oListaMunicipios del modelo 
            ModelState.Remove("oListaMunicipios");
            // Validamos el modelo
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
            // Buscamos y cargamos la informacion de la persona a eliminar
            Persona oPersona = _DBContext.Personas.Include(m => m.oMunicipio).Where(e => e.Id == idPersona).FirstOrDefault();
            return View(oPersona);

        }
        [HttpPost]
        public IActionResult EliminarPersona(Persona oPersona)
        {
            // Llamamos al procedimiento almacenado para eliminar la persona
            _DBContext.Database.ExecuteSqlRaw("CALL eliminar_persona({0})", oPersona.Id);

            return RedirectToAction("Index", "Persona");
        }
    }
}
