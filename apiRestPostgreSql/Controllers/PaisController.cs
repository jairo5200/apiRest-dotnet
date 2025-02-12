using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Common;

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
            // Creamos una lista para almacenar los países
            List<Paise> paises = new List<Paise>();

            // Llamamos al proceso almacenado
            var result = _DBContext.Paises.FromSqlRaw("SELECT * FROM obtener_paises()").ToList();

            // Asignar el resultado a la lista
            paises = result;

            //retornamos la vista con la lista de paises
            return View(paises);
        }

        [HttpGet]
        public IActionResult PaisDetalle(int idPais)
        {
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

        [HttpPost]
        public IActionResult PaisDetalle(PaisVM oPaisVM)
        {
            if (!ModelState.IsValid)
            {
                return View(oPaisVM);
            }
            _DBContext.Database.ExecuteSqlRaw("CALL guardar_pais({0}, {1})", oPaisVM.oPais.Id, oPaisVM.oPais.Nombre);
            return RedirectToAction("Index", "Pais");
        }

        [HttpGet]
        public IActionResult EliminarPais(int idPais)
        {
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
                // Llama al procedimiento almacenado
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_pais({0})", oPais.Id);

                // Redirecciona a la vista de la lista de países
                return RedirectToAction("Index", "Pais");
            }
            catch (DbException ex)
            {
                // Maneja la excepción si el país tiene departamentos asociados
                ViewData["ErrorMessage"] = ex.Message;
                return View(oPais);
            }


        }
    }
}
