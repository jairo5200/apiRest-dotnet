
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;

namespace apiRestPostgreSql.Controllers
{
    public class MunicipioController : Controller
    {
        private readonly PruebacoinkdbContext _DBContext;

        public MunicipioController(PruebacoinkdbContext context) {
            _DBContext = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Obtenemos la lista de municipios
                List<Municipio> municipios = _DBContext.Municipios
                .FromSqlRaw("SELECT * FROM obtener_municipios()")
                .ToList();

                // Iteramos la lista de municipios
                foreach (var municipio in municipios)
                {
                    // Validamos que el municipio tenga un departamento
                    if (municipio.IdDepartamento != null)
                    {
                        // Asignamos el departamento al municipio
                        municipio.oDepartamento = _DBContext.Departamentos.Find(municipio.IdDepartamento);
                    }
                }

                return View(municipios);
            }
            catch (PostgresException ex)
            {
                // Capturamos la excepción específica de PostgreSQL
                string errorMessage = $"Error: {ex.MessageText} (Código: {ex.SqlState})";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la página Error para mostrar el Error
            }
        }


        [HttpGet]
        public IActionResult MunicipioDetalle(int idMunicipio)
        {
            try
            {
                // Generamos un municipio nuevo y el listado de departamentos
                MunicipioVM oMunicipioVM = new MunicipioVM()
                {
                    oMunicipio = new Municipio(),
                    oListaDepartamento = _DBContext.Departamentos.Select(departamento => new SelectListItem()
                    {
                        Text = departamento.Nombre,
                        Value = departamento.Id.ToString()
                    }).ToList()
                };

                // Validamos que el municipio ya exista 
                if (idMunicipio != 0)
                {
                    // Llamamos a la función almacenada para obtener el municipio
                    var municipio = _DBContext.Municipios
                        .FromSqlRaw("SELECT * FROM obtener_municipio_detalle({0})", idMunicipio)
                        .AsEnumerable() // Convierte a IEnumerable para poder acceder a los resultados
                        .FirstOrDefault();
                    // Validamos que el municipio no sea null
                    if (municipio != null)
                    {
                        // Asignamos el municipio al ViewModel
                        oMunicipioVM.oMunicipio = municipio;
                    }
                }

                return View(oMunicipioVM);
            }
            catch (PostgresException ex)
            {
                // Capturamos la excepción específica de PostgreSQL
                string errorMessage = $"Error: {ex.Message} (Código: {ex.SqlState})";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la página Error para mostrar el Error
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción
                string errorMessage = $"Error: {ex.Message}";

                // Muestra un mensaje amigable al usuario
                ViewBag.ErrorMessage = errorMessage; // Asignamos el mensaje a una ViewBag
                return View("Error"); // Redirigimos a la página Error para mostrar el Error
            }
            
        }

        [HttpPost]
        public IActionResult MunicipioDetalle(MunicipioVM oMunicipioVM)
        {
            // Removemos oListaDeprtamento del modelo
            ModelState.Remove("oListaDepartamento");
            // Validamos el modelo
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, vuelve a mostrar la vista con los errores
                oMunicipioVM.oListaDepartamento = _DBContext.Departamentos.Select(departamento => new SelectListItem
                {
                    Text = departamento.Nombre,
                    Value = departamento.Id.ToString()
                }).ToList();
                return View(oMunicipioVM);
            }
            else
            {
                // Llamamos al procedimiento almacenado para guardar o actualizar el municipio
                _DBContext.Database.ExecuteSqlRaw("CALL guardar_municipio({0}, {1}, {2})",
                    oMunicipioVM.oMunicipio.Id,
                    oMunicipioVM.oMunicipio.Nombre,
                    oMunicipioVM.oMunicipio.IdDepartamento);

                return RedirectToAction("Index", "Municipio");
            }
        }

        [HttpGet]
        public IActionResult EliminarMunicipio(int idMunicipio)
        {
            // Buscamos y cargamos la informacion del municipio a eliminar
            Municipio oMunicipio = _DBContext.Municipios
                .Include(d => d.oDepartamento)
                .Where(e => e.Id == idMunicipio)
                .FirstOrDefault();
            return View(oMunicipio);

        }
        [HttpPost]
        public async Task<IActionResult> EliminarMunicipio(Municipio oMunicipio)
        {
            try
            {
                // Llamamos al procedimiento almacenado para eliminar el municipio
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_municipio({0})",oMunicipio.Id);

                return RedirectToAction("Index", "Municipio");
            }
            catch (DbException ex)
            {
                // Manejamos la excepción en caso de que el municipio tenga personas asociadas
                ViewData["ErrorMessage"] = ex.Message;
                return View(oMunicipio);
            }
        }
    }
}
