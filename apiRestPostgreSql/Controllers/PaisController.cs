using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Common;
using Npgsql;

namespace apiRestPostgreSql.Controllers
{
    public class PaisController : Controller
    {
        private readonly PruebacoinkdbContext _DBContext;
        public PaisController(PruebacoinkdbContext context)
        {
            _DBContext = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Creamos una lista para almacenar los países
                List<Paise> paises = new List<Paise>();

                // Llamamos al proceso almacenado
                var result = _DBContext.Paises.FromSqlRaw("SELECT * FROM obtener_paises()").ToList();

                // Asignamos el resultado a la lista
                paises = result;

                return View(paises);
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
        public IActionResult PaisDetalle(int idPais)
        {
            try
            {
                // Generamos un país nuevo
                PaisVM oPaisVM = new PaisVM()
                {
                    oPais = new Paise()
                };
                if (idPais != 0)
                {
                    // Llamar al stored procedure
                    oPaisVM.oPais = _DBContext.Paises
                        .FromSqlRaw("SELECT * FROM obtener_pais_por_id({0})", idPais)
                        .FirstOrDefault();
                }
                return View(oPaisVM);
            }
            catch (PostgresException ex)
            {
                // Capturamos la excepción específica de PostgreSQL
                string errorMessage = $"Error: {ex.MessageText} (Código: {ex.SqlState})";

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
        public IActionResult PaisDetalle(PaisVM oPaisVM)
        {
            // Validamos el modelo
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, vuelve a mostrar la vista con los errores
                return View(oPaisVM);
            }
            // Llamamos al procedimiento almacenado para guardar el país
            _DBContext.Database.ExecuteSqlRaw("CALL guardar_pais({0}, {1})", oPaisVM.oPais.Id, oPaisVM.oPais.Nombre);
            return RedirectToAction("Index", "Pais");
        }

        [HttpGet]
        public IActionResult EliminarPais(int idPais)
        {
            // Buscamos y cargamos la informacion del país a eliminar
            Paise oPais = _DBContext.Paises
                    .FromSqlRaw("SELECT * FROM obtener_pais_por_id({0})", idPais)
                    .FirstOrDefault();
            return View(oPais);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPais(Paise oPais)
        {
            try
            {
                // Llamamos al procedimiento almacenado
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_pais({0})", oPais.Id);

                return RedirectToAction("Index", "Pais");
            }
            catch (DbException ex)
            {
                // Manejamos la excepción si el país tiene departamentos asociados
                ViewData["ErrorMessage"] = ex.Message;
                return View(oPais);
            }


        }
    }
}
