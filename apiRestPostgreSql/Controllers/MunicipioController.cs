
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            List<Municipio> municipios = _DBContext.Municipios.Include(e => e.oDepartamento).ToList();
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
                oMunicipioVM.oMunicipio = _DBContext.Municipios.Find(idMunicipio);
            }
            return View(oMunicipioVM);
        }

        [HttpPost]
        public IActionResult MunicipioDetalle(MunicipioVM oMunicipioVM)
        {
            ModelState.Remove("oListaDepartamento");
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
                if (oMunicipioVM.oMunicipio.Id == 0)
                {
                    _DBContext.Municipios.Add(oMunicipioVM.oMunicipio);
                }
                else
                {
                    _DBContext.Municipios.Update(oMunicipioVM.oMunicipio);
                }

                _DBContext.SaveChanges();

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
            var municipio = await _DBContext.Municipios
                .AsNoTracking()
                .Include(p => p.Personas)
                .FirstOrDefaultAsync(p => p.Id == oMunicipio.Id);
            if (municipio.Personas.Any())
            {
                ViewData["ErrorMessage"] = "No se puede eliminar el Municipio porque tiene Personas asociadas.";
                return View(oMunicipio);
            }
            else
            {
                _DBContext.Municipios.Remove(oMunicipio);
                await _DBContext.SaveChangesAsync();
                return RedirectToAction("Index", "Municipio");
            }
            

        }
    }
}
