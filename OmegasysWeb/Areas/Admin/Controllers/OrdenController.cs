using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using System.Security.Claims;

namespace OmegasysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public OrdenDetalleVM ordenDetalleVM { get; set; }

        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detalle(int id)
        {
            ordenDetalleVM = new OrdenDetalleVM()
            {
                Orden = 
                    await _unidadTrabajo.Orden.obtenerPrimero(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion"),
                
                OrdenDetalleLista = 
                    await _unidadTrabajo.OrdenDetalle.obtenerTodos(o => o.OrdenId == id, incluirPropiedades: "Producto")
            };
            return View(ordenDetalleVM);
        }

        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> Procesar(int id)
        {
            var orden = await _unidadTrabajo.Orden.obtenerPrimero(o => o.Id == id);
            orden.EstadoOrden = DS.EstadoEnProceso;
            await _unidadTrabajo.Guardar();

            TempData[DS.Exitoso] = "Orden procesada, estado actualizado a En Proceso";
            return RedirectToAction("Detalle", new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> EnviarOrden(OrdenDetalleVM ordenDetalleVM)
        {
            var orden = await _unidadTrabajo.Orden.obtenerPrimero(o => o.Id == ordenDetalleVM.Orden.Id);
            orden.EstadoOrden = DS.EstadoEnviado;
            orden.Carrier = ordenDetalleVM.Orden.Carrier;
            orden.NumeroEnvio = ordenDetalleVM.Orden.NumeroEnvio;
            orden.FechaEnvio = DateTime.Now;

            await _unidadTrabajo.Guardar();

            TempData[DS.Exitoso] = "Orden procesada, estado actualizado a Enviado";
            return RedirectToAction("Detalle", new { id = ordenDetalleVM.Orden.Id });
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> obtenerOrdenLista(string estado) 
        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Orden> todos;

            if (User.IsInRole(DS.Role_Admin)) 
            {
                todos = await _unidadTrabajo.Orden.obtenerTodos(incluirPropiedades: "UsuarioAplicacion");
            }
            else
            {
                todos = await _unidadTrabajo.Orden.obtenerTodos(o => o.UsuarioAplicacionId == claim.Value, 
                                                                incluirPropiedades: "UsuarioAplicacion");
            }

            //Validación de estado
            switch (estado)
            {
                case "aprobado":
                    todos = todos.Where(o => o.EstadoOrden == DS.EstadoAprobado);
                    break;
                case "completado":
                    todos = todos.Where(o => o.EstadoOrden == DS.EstadoEnviado);
                    break;
                default:
                    break;
            }
            
            return Json(new { data = todos });
        }

        #endregion
    }
}
