using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;

namespace apiRestPostgreSql.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly PruebacoinkdbContext _DBContext;
        public DepartamentoController(PruebacoinkdbContext context)
        {
            _DBContext = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Obtenemos la lista de departamentos
                List<Departamento> departamentos = _DBContext.Departamentos
                    .FromSqlRaw("SELECT * FROM obtener_departamentos()")
                    .ToList();

                // Iteramos los departamentos
                foreach (var departamento in departamentos)
                {
                    // Validamos que el departamento tenga un país
                    if (departamento.IdPais != null)
                    {
                        // Asignamos el país al departamento
                        departamento.oPais = _DBContext.Paises.Find(departamento.IdPais);
                    }
                }

                return View(departamentos);
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
        public IActionResult DepartamentoDetalle(int idDepartamento)
        {
            try
            {
                // Generamos un departamento nuevo y el listado de países
                DepartamentoVM oDepartamentoVM = new DepartamentoVM()
                {
                    oDepartamento = new Departamento(),
                    oListaPais = _DBContext.Paises.Select(pais => new SelectListItem()
                    {
                        Text = pais.Nombre,
                        Value = pais.Id.ToString()
                    }).ToList()
                };

                // Validamos que el departamento ya exista
                if (idDepartamento != 0)
                {
                    // Llamamos a la función almacenada para obtener el departamento
                    var departamento = _DBContext.Departamentos
                        .FromSqlRaw("SELECT * FROM obtener_departamento_por_id({0})", idDepartamento)
                        .AsEnumerable() // Convierte a IEnumerable para poder acceder a los resultados
                        .FirstOrDefault();
                    // Validamos que el departamento no sea null
                    if (departamento != null)
                    {
                        // Asignamos el departamento al ViewModel
                        oDepartamentoVM.oDepartamento = departamento;
                    }
                }

                return View(oDepartamentoVM);
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
        public IActionResult DepartamentoDetalle(DepartamentoVM oDepartamentoVM)
        {
            // Removemos oListaPais del modelo
            ModelState.Remove("oListaPais");
            // Validamos el modelo
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, vuelve a mostrar la vista con los errores
                oDepartamentoVM.oListaPais = _DBContext.Paises.Select(pais => new SelectListItem
                {
                    Text = pais.Nombre,
                    Value = pais.Id.ToString()
                }).ToList();
                return View(oDepartamentoVM);
            }
            else
            {
                // Llamamos al procedimiento almacenado para guardar el departamento
                _DBContext.Database.ExecuteSqlRaw("CALL guardar_departamento({0}, {1}, {2})",
                    oDepartamentoVM.oDepartamento.Id,
                    oDepartamentoVM.oDepartamento.Nombre,
                    oDepartamentoVM.oDepartamento.IdPais);

                return RedirectToAction("Index", "Departamento");
            }

        }

        [HttpGet]
        public IActionResult EliminarDepartamento(int idDepartamento)
        {
            // Buscamos y cargamos la informacion del departamento a eliminar
            Departamento oDepartamento = _DBContext.Departamentos.Include(p => p.oPais).Where(e => e.Id == idDepartamento).FirstOrDefault();
            return View(oDepartamento);

        }
        [HttpPost]
        public async Task<IActionResult> EliminarDepartamento(Departamento oDepartamento)
        {
            try
            {
                // Llamamos al procedimiento almacenado para eliminar el departamento
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_departamento({0})", oDepartamento.Id);

                return RedirectToAction("Index", "Departamento");
            }
            catch (Exception ex)
            {
                // Manejamos la excepción si el departamento tiene municipios asociados
                ViewData["ErrorMessage"] = ex.Message;
                return View(oDepartamento);
            }
        }
    }
}