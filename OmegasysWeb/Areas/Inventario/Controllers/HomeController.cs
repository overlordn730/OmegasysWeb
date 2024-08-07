using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.Especificaciones;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using System.Diagnostics;
using System.Security.Claims;

namespace OmegasysWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        private CarroCompraVM carroCompraVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
        }

        //public async Task<IActionResult> Index()
        //{
        //    IEnumerable<Producto> productoLista = await _unidadTrabajo.Producto.obtenerTodos();
        //    return View(productoLista);
        //}
        
        public async Task<IActionResult> Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {
            // Se llena la sesión
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var carroLista = await _unidadTrabajo.CarroCompra.obtenerTodos(x => x.UsuarioAplicacionId == claim.Value);
                var numeroProductos = carroLista.Count(); // Numero de registros que corresponden a el user

                HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos);
            }

            if (!String.IsNullOrEmpty(busqueda))
                pageNumber = 1;
            else
                busqueda = busquedaActual;

            ViewData["BusquedaActual"] = busqueda;

            if (pageNumber < 1) pageNumber = 1;

            Parametros parametros = new Parametros()
            { 
                PageNumber = pageNumber,
                PageSize = 4
            };

            var resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros);

            if (!String.IsNullOrEmpty(busqueda))
                resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros, p => p.Descripcion.Contains(busqueda));

            ViewData["TotalPaginas"] = resultado.Metadata.TotalPages;
            ViewData["TotalRegistros"] = resultado.Metadata.TotalCount;
            ViewData["PageSize"] = resultado.Metadata.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Anterior"] = "disabled";
            ViewData["Siguiente"] = "";

            if (pageNumber > 1) ViewData["Anterior"] = "";

            if (resultado.Metadata.TotalPages <= pageNumber) ViewData["Siguiente"] = "disabled";

            return View(resultado);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            carroCompraVM = new CarroCompraVM();
            carroCompraVM.Compania = await _unidadTrabajo.Compania.obtenerPrimero();
            carroCompraVM.Producto = await _unidadTrabajo.Producto.obtenerPrimero(p => p.Id == id, incluirPropiedades:"Marca,Categoria");

            var bodegaProducto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(x => x.ProductoId == id && x.BodegaId == carroCompraVM.Compania.BodegaVentaId);

            if (bodegaProducto == null)
            {
                carroCompraVM.Stock = 0;
            }
            else
            {
                carroCompraVM.Stock = bodegaProducto.Cantidad;
            }

            carroCompraVM.CarroCompra = new CarroCompra()
            {
                Producto = carroCompraVM.Producto,
                ProductoId = carroCompraVM.Producto.Id
            };

            return View(carroCompraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detalle(CarroCompraVM carroCompraVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM.CarroCompra.UsuarioAplicacionId = claim.Value;

            CarroCompra carroBD = await _unidadTrabajo.CarroCompra.obtenerPrimero(c => c.UsuarioAplicacionId == claim.Value &&
                                                                                  c.ProductoId == carroCompraVM.CarroCompra.ProductoId);

            if (carroBD == null)
            {
                await _unidadTrabajo.CarroCompra.agregar(carroCompraVM.CarroCompra);
            }
            else
            {
                carroBD.Cantidad += carroCompraVM.CarroCompra.Cantidad;
                _unidadTrabajo.CarroCompra.actualizar(carroBD);
            }

            await _unidadTrabajo.Guardar();
            TempData[DS.Exitoso] = "Producto agregado al carro de compras";

            // Agregar valor a la sesion
            var carroLista = await _unidadTrabajo.CarroCompra.obtenerTodos(x => x.UsuarioAplicacionId == claim.Value);
            var numeroProductos = carroLista.Count(); // Numero de registros que corresponden a el user

            HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
