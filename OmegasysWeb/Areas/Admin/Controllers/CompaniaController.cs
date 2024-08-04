using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using System.Security.Claims;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CompaniaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CompaniaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert()
        {
            CompaniaVM companiaVM = new CompaniaVM()
            {
                Compania = new Modelos.Compania(),
                BodegaLista = _unidadTrabajo.Inventario.obtenerTodosDropdownLista("Bodega")
            };

            companiaVM.Compania = await _unidadTrabajo.Compania.obtenerPrimero();

            if (companiaVM.Compania == null)
            {
                companiaVM.Compania = new Modelos.Compania();
            }

            return View(companiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompaniaVM companiaVM)
        {
            if (ModelState.IsValid)
            {
                TempData[DS.Exitoso] = "Compania grabada exitosamente";
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (companiaVM.Compania.Id == 0 )
                {
                    companiaVM.Compania.CreadoPorId = claim.Value;
                    companiaVM.Compania.ActualizadoPorId = claim.Value;
                    companiaVM.Compania.FechaCreacion = DateTime.Now;
                    companiaVM.Compania.FechaActualizacion = DateTime.Now;
                    await _unidadTrabajo.Compania.agregar(companiaVM.Compania);
                }
                else
                {
                    companiaVM.Compania.ActualizadoPorId = claim.Value;
                    companiaVM.Compania.FechaActualizacion = DateTime.Now;
                    _unidadTrabajo.Compania.actualizar(companiaVM.Compania);
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index", "Home", new { area = "Inventario"});
            }
            TempData[DS.Fallido] = "Ups, algo salió mal";
            return View(companiaVM);
        }
    }
}
