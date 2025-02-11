using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<Departamento> departamentos = _DBContext.Departamentos.Include(e => e.oPais).ToList();
            return View(departamentos);
        }

        [HttpGet]
        public IActionResult DepartamentoDetalle(int idDepartamento)
        {
            DepartamentoVM oDepartamentoVM = new DepartamentoVM()
            {
                oDepartamento = new Departamento(),
                oListaPais = _DBContext.Paises.Select(pais => new SelectListItem(){
                    Text = pais.Nombre,
                    Value = pais.Id.ToString()
                }).ToList()
            };

            if (idDepartamento != 0)
            {
               oDepartamentoVM.oDepartamento = _DBContext.Departamentos.Find(idDepartamento);
            }
            return View(oDepartamentoVM);
        }

        [HttpPost]
        public IActionResult DepartamentoDetalle(DepartamentoVM oDepartamentoVM)
        {
            ModelState.Remove("oListaPais");
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
                if (oDepartamentoVM.oDepartamento.Id == 0)
                {
                    _DBContext.Departamentos.Add(oDepartamentoVM.oDepartamento);
                }
                else
                {
                    _DBContext.Departamentos.Update(oDepartamentoVM.oDepartamento);
                }

                _DBContext.SaveChanges();

                return RedirectToAction("Index", "Departamento");
            }
            
        }

        [HttpGet]
        public IActionResult EliminarDepartamento(int idDepartamento)
        {
            Departamento oDepartamento = _DBContext.Departamentos.Include(p => p.oPais).Where(e => e.Id == idDepartamento).FirstOrDefault();
            return View(oDepartamento);

        }
        [HttpPost]
        public IActionResult EliminarDepartamento(Departamento oDepartamento)
        {

            _DBContext.Departamentos.Remove(oDepartamento);
            _DBContext.SaveChanges();
            return RedirectToAction("Index", "Departamento");

        }
    }
}
