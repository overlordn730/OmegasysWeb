using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.Utilidades
{
    public static class DS
    {
        public const string Exitoso = "Proceso exitoso";
        public const string Fallido = "Proceso fallido";

        public const string ImagenRuta = @"\imagenes\producto\";

        public const string Role_Admin = "Admin";
        public const string Role_Cliente = "Cliente";
        public const string Role_Inventario = "Inventario";

        public const string ssCarroCompra = "Sesion Carro Compras";

        // Estados de la Orden
        public const string EstadoPendiente = "Pendiente";
        public const string EstadoAprobado = "Aprobado";
        public const string EstadoEnProceso = "Procesando";
        public const string EstadoEnviado = "Enviado";
        public const string EstadoCancelado = "Cancelado";
        public const string EstadoDevuelto = "Devuelto";

        //Estados del Pago de la Orden
        public const string PagoEstadoDevuelto = "Devuelto";
        public const string PagoEstadoPendiente = "Pendiente";
        public const string PagoEstadoAprobado = "Aprobado";
        public const string PagoEstadoRetrasado = "Retrasado";
        public const string PagoEstadoAtrasado = "Atrasado";
        public const string PagoEstadoRechazado = "Rechazado";
    }
}
