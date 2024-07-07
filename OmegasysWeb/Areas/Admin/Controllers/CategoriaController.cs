using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Utilidades;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            this._unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if (id == null)
            {
                categoria.Estado = true;
                return View(categoria);
            }

            categoria = await _unidadTrabajo.Categoria.obtener(id.GetValueOrDefault());

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if(categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.agregar(categoria);
                    TempData[DS.Exitoso] = "Categoria creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.actualizar(categoria);
                    TempData[DS.Exitoso] = "Categoria actualizada exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
                    TempData[DS.Exitoso] = "Error al procesar";
            return View(categoria);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.obtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriadb = await _unidadTrabajo.Categoria.obtener(id);
            if (categoriadb is null)
            {
                return Json(new { success = false, message = "Error al borrar la categoria" });
            }
            _unidadTrabajo.Categoria.remover(categoriadb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        [ActionName("validarNombre")]
        public async Task<IActionResult> validarNombre(string nombre, int? id)
        {
            bool valor = false;
            var list = await _unidadTrabajo.Categoria.obtenerTodos();

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
