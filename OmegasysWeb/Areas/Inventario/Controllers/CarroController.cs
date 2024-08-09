using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using System.Security.Claims;

namespace OmegasysWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }

        public CarroController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM();
            carroCompraVM.Orden = new Modelos.Orden();
            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.obtenerTodos(u => u.UsuarioAplicacionId == claim.Value, incluirPropiedades:"Producto");

            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio; // Siempre le va a cargar el precio actual del producto
                carroCompraVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            return View(carroCompraVM);
        }

        public async Task<IActionResult> mas(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.obtenerPrimero(x => x.Id == carroId);
            carroCompras.Cantidad += 1;
            await _unidadTrabajo.Guardar();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> menos(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.obtenerPrimero(x => x.Id == carroId);
            
            if (carroCompras.Cantidad == 1)
            {
                // Eliminamos el registro y actualizamos la sesion
                var carroLista = await _unidadTrabajo.CarroCompra.obtenerTodos(x => x.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);
                var numeroProductos = carroLista.Count();
                _unidadTrabajo.CarroCompra.remover(carroCompras);
                await _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos - 1);
            }
            else
            {
                carroCompras.Cantidad -= 1;
                await _unidadTrabajo.Guardar();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> remover(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.obtenerPrimero(x => x.Id == carroId);

            //Remueve el registro del Carro de Compras y Actualiza la sesion
            var carroLista = await _unidadTrabajo.CarroCompra.obtenerTodos(x => x.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);
            var numeroProductos = carroLista.Count();
            _unidadTrabajo.CarroCompra.remover(carroCompras);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos - 1);
            return RedirectToAction("Index");
        }
    }
}
