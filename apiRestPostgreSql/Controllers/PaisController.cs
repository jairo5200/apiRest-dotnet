using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<Paise> paises = _DBContext.Paises.ToList();
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
                oPaisVM.oPais = _DBContext.Paises.Find(idPais);
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
            if (oPaisVM.oPais.Id == 0)
            {
                _DBContext.Paises.Add(oPaisVM.oPais);
            }
            else
            {
                _DBContext.Paises.Update(oPaisVM.oPais);
            }

            _DBContext.SaveChanges();

            return RedirectToAction("Index", "Pais");
        }

        [HttpGet]
        public IActionResult EliminarPais(int idPais)
        {
            Paise oPais = _DBContext.Paises.Where(e => e.Id == idPais).FirstOrDefault();
            return View(oPais);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPais(Paise oPais)
        {
            var pais = await _DBContext.Paises
                .AsNoTracking()
                .Include(p => p.Departamentos)
                .FirstOrDefaultAsync(p => p.Id == oPais.Id);
            if (pais.Departamentos.Any())
            {
                ViewData["ErrorMessage"] = "No se puede eliminar el país porque tiene departamentos asociados.";
                return View(oPais);
            }
            else
            {
                _DBContext.Paises.Remove(oPais);
                _DBContext.SaveChangesAsync();
                return RedirectToAction("Index", "Pais");
            }
            

        }
    }
}
