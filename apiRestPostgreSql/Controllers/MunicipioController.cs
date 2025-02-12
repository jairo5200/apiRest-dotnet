
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            List<Municipio> municipios = _DBContext.Municipios
                .FromSqlRaw("SELECT * FROM obtener_municipios()")
                .ToList();

            foreach (var municipio in municipios)
            {
                if (municipio.IdDepartamento != null)
                {
                    municipio.oDepartamento = _DBContext.Departamentos.Find(municipio.IdDepartamento);
                }
            }

            return View(municipios);
        }


        [HttpGet]
        public IActionResult MunicipioDetalle(int idMunicipio)
        {
            MunicipioVM oMunicipioVM = new MunicipioVM()
            {
                oMunicipio = new Municipio(),
                oListaDepartamento = _DBContext.Departamentos.Select(departamento => new SelectListItem()
                {
                    Text = departamento.Nombre,
                    Value = departamento.Id.ToString()
                }).ToList()
            };

            if (idMunicipio != 0)
            {
                // Llamar a la función almacenada para obtener el municipio
                var municipio = _DBContext.Municipios
                    .FromSqlRaw("SELECT * FROM obtener_municipio_detalle({0})", idMunicipio)
                    .AsEnumerable() // Convierte a IEnumerable para poder acceder a los resultados
                    .FirstOrDefault();

                if (municipio != null)
                {
                    oMunicipioVM.oMunicipio = municipio;
                }
            }

            return View(oMunicipioVM);
        }

        [HttpPost]
        public IActionResult MunicipioDetalle(MunicipioVM oMunicipioVM)
        {
            ModelState.Remove("oListaDepartamento");
            if (!ModelState.IsValid)
            {
                oMunicipioVM.oListaDepartamento = _DBContext.Departamentos.Select(departamento => new SelectListItem
                {
                    Text = departamento.Nombre,
                    Value = departamento.Id.ToString()
                }).ToList();
                return View(oMunicipioVM);
            }
            else
            {
                // Llamar al procedimiento almacenado para guardar o actualizar el municipio
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
                // Llamar al procedimiento almacenado para eliminar el municipio
                _DBContext.Database.ExecuteSqlRaw("CALL eliminar_municipio({0})",oMunicipio.Id);

                return RedirectToAction("Index", "Municipio");
            }
            catch (DbException ex)
            {
                // Manejar la excepción en caso de que el municipio tenga personas asociadas
                ViewData["ErrorMessage"] = ex.Message;
                return View(oMunicipio);
            }
        }
    }
}
