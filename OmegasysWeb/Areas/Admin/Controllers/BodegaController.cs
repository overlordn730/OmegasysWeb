using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Utilidades;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            this._unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Bodega bodega = new Bodega();

            if (id == null)
            {
                bodega.Estado = true;
                return View(bodega);
            }

            bodega = await _unidadTrabajo.Bodega.obtener(id.GetValueOrDefault());

            if (bodega == null)
            {
                return NotFound();
            }

            return View(bodega);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if(bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.agregar(bodega);
                    TempData[DS.Exitoso] = "Bodega creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.actualizar(bodega);
                    TempData[DS.Exitoso] = "Bodega actualizada exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
                    TempData[DS.Exitoso] = "Error al procesar";
            return View(bodega);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.obtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegadb = await _unidadTrabajo.Bodega.obtener(id);
            if (bodegadb is null)
            {
                return Json(new { success = false, message = "Error al borrar la Bodega" });
            }
            _unidadTrabajo.Bodega.remover(bodegadb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }

        [ActionName("validarNombre")]
        public async Task<IActionResult> validarNombre(string nombre, int? id)
        {
            bool valor = false;
            var list = await _unidadTrabajo.Bodega.obtenerTodos();

            if (id == 0)
            {
                valor = list.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = list.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }

            if(valor){
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }
        #endregion
    }
}
