using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.Especificaciones;
using OmegasysWeb.Modelos.ViewModels;
using System.Diagnostics;

namespace OmegasysWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;

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
        
        public IActionResult Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {
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
