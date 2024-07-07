using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Utilidades;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            this._unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Marca marca = new Marca();

            if (id == null)
            {
                marca.Estado = true;
                return View(marca);
            }

            marca = await _unidadTrabajo.Marca.obtener(id.GetValueOrDefault());

            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if(marca.Id == 0)
                {
                    await _unidadTrabajo.Marca.agregar(marca);
                    TempData[DS.Exitoso] = "Marca creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Marca.actualizar(marca);
                    TempData[DS.Exitoso] = "Marca actualizada exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
                    TempData[DS.Exitoso] = "Error al procesar";
            return View(marca);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos = await _unidadTrabajo.Marca.obtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var marcadb = await _unidadTrabajo.Marca.obtener(id);
            if (marcadb is null)
            {
                return Json(new { success = false, message = "Error al borrar la marca" });
            }
            _unidadTrabajo.Marca.remover(marcadb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca borrada exitosamente" });
        }

        [ActionName("validarNombre")]
        public async Task<IActionResult> validarNombre(string nombre, int? id)
        {
            bool valor = false;
            var list = await _unidadTrabajo.Marca.obtenerTodos();

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
