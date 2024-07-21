using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using System.Security.Claims;

namespace OmegasysWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventario)]
    public class InventarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public InventarioVM inventarioVM { get; set; }

        public InventarioController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NuevoInventario()
        {
            inventarioVM = new InventarioVM()
            {
                Inventario = new Modelos.Inventario(),
                BodegaLista = _unidadTrabajo.Inventario.obtenerTodosDropdownLista("Bodega")
            };

            inventarioVM.Inventario.Estado = false;
            // Obtenemos el usuario
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            inventarioVM.Inventario.UsuarioAplicacionId = claim.Value;
            inventarioVM.Inventario.FechaInicial = DateTime.Now;
            inventarioVM.Inventario.FechaFinal = DateTime.Now;

            return View(inventarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoInventario(InventarioVM inventarioVM)
        {
            if (ModelState.IsValid)
            {
                inventarioVM.Inventario.FechaInicial = DateTime.Now;
                inventarioVM.Inventario.FechaFinal = DateTime.Now;
                await _unidadTrabajo.Inventario.agregar(inventarioVM.Inventario);
                await _unidadTrabajo.Guardar();
                return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
            }
            
            inventarioVM.BodegaLista = _unidadTrabajo.Inventario.obtenerTodosDropdownLista("Bodega");

            return View(inventarioVM);
        }

        public async Task<IActionResult> DetalleInventario(int id)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.obtenerPrimero(x => x.Id == id, incluirPropiedades: "Bodega");
            inventarioVM.InventarioDetalles = await _unidadTrabajo.InventarioDetalle.obtenerTodos(d => d.InventarioId == id,
                                                                                            incluirPropiedades:"Producto,Producto.Marca");
            return View(inventarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetalleInventario(int inventarioId, int productoId, int cantidadId)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.obtenerPrimero(i => i.Id == inventarioId);
            var bodegaProducto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(bp => bp.ProductoId == productoId && bp.BodegaId == inventarioVM.Inventario.BodegaId);
            var detalle = await _unidadTrabajo.InventarioDetalle.obtenerPrimero(d => d.InventarioId == inventarioId && d.ProductoId == productoId);

            if (detalle == null)
            {
                inventarioVM.InventarioDetalle = new InventarioDetalle();
                inventarioVM.InventarioDetalle.ProductoId = productoId;
                inventarioVM.InventarioDetalle.InventarioId = inventarioId;

                if (bodegaProducto != null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = bodegaProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
                }
                inventarioVM.InventarioDetalle.Cantidad = cantidadId;
                await _unidadTrabajo.InventarioDetalle.agregar(inventarioVM.InventarioDetalle);
                await _unidadTrabajo.Guardar();
            }
            else
            {
                detalle.Cantidad += cantidadId;
                await _unidadTrabajo.Guardar();
            }
            return RedirectToAction("DetalleInventario", new { id = inventarioId });
        }

        public async Task<IActionResult> Mas(int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.obtener(id);
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.obtener(detalle.InventarioId);

            detalle.Cantidad += 1;
            await _unidadTrabajo.Guardar();
            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }
        public async Task<IActionResult> Menos(int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.obtener(id);
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.obtener(detalle.InventarioId);

            if (detalle.Cantidad == 1){
                _unidadTrabajo.InventarioDetalle.remover(detalle);
                await _unidadTrabajo.Guardar();
            }
            else
            {
                detalle.Cantidad -= 1;
                await _unidadTrabajo.Guardar();
            }
            
            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }

        public async Task<IActionResult> GenerarStock(int id)
        {
            var inventario = await _unidadTrabajo.Inventario.obtener(id);
            var detalleLista = await _unidadTrabajo.InventarioDetalle.obtenerTodos(d => d.InventarioId == id);
            // Obtenemos el usuario
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var item in detalleLista)
            {
                var bodegaProducto = new BodegaProducto();
                bodegaProducto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(b => b.ProductoId == item.ProductoId &&
                                                                                    b.BodegaId == inventario.BodegaId);

                // El registro de stock existe, se debe actualizar
                if (bodegaProducto != null)
                {
                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id,
                                                                          "Entrada",
                                                                          "Registro de Inventario",
                                                                          bodegaProducto.Cantidad,
                                                                          item.Cantidad,
                                                                          claim.Value);
                    bodegaProducto.Cantidad += item.Cantidad;
                    await _unidadTrabajo.Guardar();
                }
                else
                {
                    // No existe stock, por lo tanto se crea
                    bodegaProducto = new BodegaProducto();
                    bodegaProducto.BodegaId = inventario.BodegaId;
                    bodegaProducto.ProductoId = item.ProductoId;
                    bodegaProducto.Cantidad = item.Cantidad;
                    await _unidadTrabajo.BodegaProducto.agregar(bodegaProducto);
                    await _unidadTrabajo.Guardar();
                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id,
                                                                          "Entrada",
                                                                          "Inventario Inicial",
                                                                          0,
                                                                          item.Cantidad,
                                                                          claim.Value);
                }
            }

            //Actualizamos la cabecera del inventario
            inventario.Estado = true;
            inventario.FechaFinal = DateTime.Now;
            await _unidadTrabajo.Guardar();
            TempData[DS.Exitoso] = "Stock Registrado Exitosamente";
            return RedirectToAction("Index");
        }

        public IActionResult KardexProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProducto(string fechaInicioId, string fechaFinalId, int productoId)
        {
            return RedirectToAction("KardexProductoResultado", new { fechaInicioId, fechaFinalId, productoId });
        }

        public async Task<IActionResult> KardexProductoResultado(string fechaInicioId, string fechaFinalId, int productoId)
        {
            KardexInventarioVM kardexInventarioVM = new KardexInventarioVM();
            kardexInventarioVM.Producto = new Producto();
            kardexInventarioVM.Producto = await _unidadTrabajo.Producto.obtener(productoId);

            kardexInventarioVM.FechaInicio = DateTime.Parse(fechaInicioId);
            kardexInventarioVM.FechaFinal = DateTime.Parse(fechaFinalId).AddHours(23).AddMinutes(59);

            kardexInventarioVM.KardexInventarioLista = await _unidadTrabajo.KardexInventario.obtenerTodos(k => k.BodegaProducto.ProductoId == productoId &&
                                                                                                          (k.FechaRegistro >= kardexInventarioVM.FechaInicio &&
                                                                                                          k.FechaRegistro <= kardexInventarioVM.FechaFinal),
                                                                                                          incluirPropiedades: "BodegaProducto,BodegaProducto.Producto,BodegaProducto.Bodega",
                                                                                                          orderBy: o => o.OrderBy(o => o.FechaRegistro));

            return View(kardexInventarioVM);
        }

        #region API

        [HttpGet]
        public async Task<ActionResult> obtenerTodos()
        {
            var todos = await _unidadTrabajo.BodegaProducto.obtenerTodos(incluirPropiedades: "Bodega,Producto");
            return Json(new { data = todos });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProducto(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var listaproductos = await _unidadTrabajo.Producto.obtenerTodos(p => p.Estado == true);
                var data = listaproductos.Where(d => d.NumeroSerie.Contains(term, StringComparison.OrdinalIgnoreCase) 
                                                  || d.Descripcion.Contains(term, StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                return Ok(data);
            }
            return Ok();
        }

        #endregion
    }
}
