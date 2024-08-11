using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using OmegasysWeb.Modelos.ViewModels;
using OmegasysWeb.Utilidades;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Stripe.Identity;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace OmegasysWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly string _webUrl;

        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }

        public CarroController(IUnidadTrabajo unidadTrabajo, IConfiguration configuration)
        {
            _unidadTrabajo = unidadTrabajo;
            _webUrl = configuration.GetValue<string>("DomainUrls:WEB_URL");
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

        public async Task<IActionResult> Proceder()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM()
            {
                Orden = new Modelos.Orden(),
                CarroCompraLista = await _unidadTrabajo.CarroCompra.obtenerTodos(c => c.UsuarioAplicacionId == claim.Value,
                                                                                 incluirPropiedades: "Producto"),
                Compania = await _unidadTrabajo.Compania.obtenerPrimero()
            };

            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacion = await _unidadTrabajo.UsuarioAplicacion.obtenerPrimero(u => u.Id == claim.Value);

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            carroCompraVM.Orden.NombresCliente = carroCompraVM.Orden.UsuarioAplicacion.Nombres + " " + 
                                                 carroCompraVM.Orden.UsuarioAplicacion.Apellidos;

            carroCompraVM.Orden.Telefono = carroCompraVM.Orden.UsuarioAplicacion.PhoneNumber;
            carroCompraVM.Orden.Direccion = carroCompraVM.Orden.UsuarioAplicacion.Direccion;
            carroCompraVM.Orden.Pais = carroCompraVM.Orden.UsuarioAplicacion.Pais;
            carroCompraVM.Orden.Ciudad = carroCompraVM.Orden.UsuarioAplicacion.Ciudad;

            //Control Stock
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                // Capturar el stock del producto
                var producto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(b => b.ProductoId == lista.ProductoId &&
                                                                                  b.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Fallido] = 
                        "La cantidad del producto "+lista.Producto.Descripcion + " excede al stock actual ("+producto.Cantidad+")";
                    return RedirectToAction("Index");
                }
            }
            return View(carroCompraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceder(CarroCompraVM carroCompraVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.obtenerTodos
                                                                (c => c.UsuarioAplicacionId == claim.Value,
                                                                incluirPropiedades:"Producto");

            carroCompraVM.Compania = await _unidadTrabajo.Compania.obtenerPrimero();
            carroCompraVM.Orden.TotalOrden = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;
            carroCompraVM.Orden.FechaOrden = DateTime.Now;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }


            //Control Stock
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                // Capturar el stock del producto
                var producto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(b => b.ProductoId == lista.ProductoId &&
                                                                                  b.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Fallido] =
                        "La cantidad del producto " + lista.Producto.Descripcion + " excede al stock actual (" + producto.Cantidad + ")";
                    return RedirectToAction("Index");
                }
            }
            carroCompraVM.Orden.EstadoOrden = DS.EstadoPendiente;
            carroCompraVM.Orden.EstadoPago = DS.PagoEstadoPendiente;
            await _unidadTrabajo.Orden.agregar(carroCompraVM.Orden);
            await _unidadTrabajo.Guardar();

            //Guardamos el detalle
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProductoId = lista.ProductoId,
                    OrdenId = carroCompraVM.Orden.Id,
                    Precio = lista.Precio,
                    Cantidad = lista.Cantidad
                };
                await _unidadTrabajo.OrdenDetalle.agregar(ordenDetalle);
                await _unidadTrabajo.Guardar();
            }

            var usuario = await _unidadTrabajo.UsuarioAplicacion.obtenerPrimero(u => u.Id == claim.Value);

            //Stripe
            var options = new SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"inventario/carro/OrdenConfirmacion?id={carroCompraVM.Orden.Id}",
                CancelUrl = _webUrl + "inventario/carro/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = usuario.Email,
                PaymentMethodTypes = new List<string> { "card" }
            };

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                var sessionLineItem = new SessionLineItemOptions {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(lista.Precio * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = lista.Producto.Descripcion
                        }
                    },
                    Quantity = lista.Cantidad
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unidadTrabajo.Orden.ActualizarPagoStripeId(carroCompraVM.Orden.Id, session.Id, session.PaymentIntentId);
            await _unidadTrabajo.Guardar();

            // Redirect to Stripe
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrdenConfirmacion(int id)
        {
            var orden = await _unidadTrabajo.Orden.obtenerPrimero(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion");
            var service = new SessionService();
            Session session = service.Get(orden.SessionId);

            var carroCompra = await _unidadTrabajo.CarroCompra.obtenerTodos
                                                    (u => u.UsuarioAplicacionId == orden.UsuarioAplicacionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unidadTrabajo.Orden.ActualizarPagoStripeId(id, session.Id, session.PaymentIntentId);
                _unidadTrabajo.Orden.ActualizarEstado(id, DS.EstadoAprobado, DS.PagoEstadoAprobado);
                await _unidadTrabajo.Guardar();

                // Disminuir Stock de la bodega de venta
                var compania = await _unidadTrabajo.Compania.obtenerPrimero();
                
                foreach (var lista in carroCompra)
                {
                    var bodegaProducto = new BodegaProducto();
                    bodegaProducto = await _unidadTrabajo.BodegaProducto.obtenerPrimero(b => b.ProductoId == lista.ProductoId &&
                                                                                        b.BodegaId == compania.BodegaVentaId);

                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id, "Salida",
                                                                          "Venta - Orden Nro. " + id,
                                                                          bodegaProducto.Cantidad,
                                                                          lista.Cantidad,
                                                                          orden.UsuarioAplicacionId);

                    bodegaProducto.Cantidad -= lista.Cantidad;
                    await _unidadTrabajo.Guardar();
                }
            }

            // Borramos el carro de compras y la sesion del carro de compras
            List<CarroCompra> carroCompraLista = carroCompra.ToList();
            _unidadTrabajo.CarroCompra.removerRango(carroCompraLista);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompra, 0);

            return View(id);
        }
    }
}
