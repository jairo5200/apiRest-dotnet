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
            List<Departamento> departamentos = _DBContext.Departamentos
         .FromSqlRaw("SELECT * FROM obtener_departamentos()")
         .ToList();

            foreach (var departamento in departamentos)
            {
                if (departamento.IdPais != null)
                {
                    departamento.oPais = _DBContext.Paises.Find(departamento.IdPais);
                }
            }

            return View(departamentos);
        }

        [HttpGet]
        public IActionResult DepartamentoDetalle(int idDepartamento)
        {
            DepartamentoVM oDepartamentoVM = new DepartamentoVM()
            {
                oDepartamento = new Departamento(),
                oListaPais = _DBContext.Paises.Select(pais => new SelectListItem()
                {
                    Text = pais.Nombre,
                    Value = pais.Id.ToString()
                }).ToList()
            };

            if (idDepartamento != 0)
            {
                // Llamar a la función almacenada para obtener el departamento
                var departamento = _DBContext.Departamentos
                    .FromSqlRaw("SELECT * FROM obtener_departamento_por_id({0})", idDepartamento)
                    .AsEnumerable() // Convierte a IEnumerable para poder acceder a los resultados
                    .FirstOrDefault();

                if (departamento != null)
                {
                    oDepartamentoVM.oDepartamento = departamento;
                }
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
                // Llamar al procedimiento almacenado para guardar el departamento
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
            Departamento oDepartamento = _DBContext.Departamentos.Include(p => p.oPais).Where(e => e.Id == idDepartamento).FirstOrDefault();
            return View(oDepartamento);

        }
        [HttpPost]
        public async Task<IActionResult> EliminarDepartamento(Departamento oDepartamento)
        {
            try
            {
                // Llamar al procedimiento almacenado para eliminar el departamento
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_departamento({0})", oDepartamento.Id);

                return RedirectToAction("Index", "Departamento");
            }
            catch (Exception ex)
            {
                // Manejar la excepción si el departamento tiene municipios asociados
                ViewData["ErrorMessage"] = ex.Message;
                return View(oDepartamento);
            }
        }
    }
}