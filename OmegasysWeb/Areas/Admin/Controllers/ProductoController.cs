using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            this._unidadTrabajo = unidadTrabajo;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.obtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.obtenerTodosDropdownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.obtenerTodosDropdownList("Producto")
            };
            
            if (id == null)
            {
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = await _unidadTrabajo.Producto.obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productoVM.Producto.Id == 0)
                {
                    string upload = webRootPath + DS.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productoVM.Producto.ImagenURL = fileName + extension;
                    await _unidadTrabajo.Producto.agregar(productoVM.Producto);
                }
                else
                {
                    var objProducto = await _unidadTrabajo.Producto.obtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var anteriorFile = Path.Combine(upload, objProducto.ImagenURL);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenURL = fileName + extension;
                    }
                    else
                    {
                        productoVM.Producto.ImagenURL = objProducto.ImagenURL;
                    }
                    _unidadTrabajo.Producto.actualizar(productoVM.Producto);
                }
                TempData[DS.Exitoso] = "Proceso exitoso";
                await _unidadTrabajo.Guardar();
                //return View("Index");
                return RedirectToAction(nameof(Index));
            }
            productoVM.CategoriaLista = _unidadTrabajo.Producto.obtenerTodosDropdownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.obtenerTodosDropdownList("Marca");
            productoVM.PadreLista= _unidadTrabajo.Producto.obtenerTodosDropdownList("Marca");
            //return View(productoVM);
            return RedirectToAction(nameof(productoVM));
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.obtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productodb = await _unidadTrabajo.Producto.obtener(id);
            if (productodb is null)
            {
                return Json(new { success = false, message = "Error al borrar el producto" });
            }

            // Remover imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productodb.ImagenURL);

            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            _unidadTrabajo.Producto.remover(productodb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrada exitosamente" });
        }

        [ActionName("validarSerie")]
        public async Task<IActionResult> validarSerie(string serie, int? id)
        {
            bool valor = false;
            var list = await _unidadTrabajo.Producto.obtenerTodos();

            if (id == 0)
            {
                valor = list.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
            else
            {
                valor = list.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id != id);
            }

            if(valor){
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }
        #endregion
    }
}
