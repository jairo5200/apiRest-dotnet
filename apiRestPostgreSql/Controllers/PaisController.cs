using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult EliminarPais(Paise oPais)
        {
            _DBContext.Paises.Remove(oPais);
            _DBContext.SaveChanges();
            return RedirectToAction("Index", "Pais");

        }
    }
}
