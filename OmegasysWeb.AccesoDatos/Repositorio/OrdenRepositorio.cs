using OmegasysWeb.AccesoDatos.Data;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using OmegasysWeb.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio
{
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public OrdenRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(Orden orden)
        {
            _dbContext.Update(orden);
        }

        public void ActualizarEstado(int id, string ordenEstado, string pagoEstado)
        {
            var ordenDB = _dbContext.Ordenes.FirstOrDefault(o => o.Id == id);

            if (ordenDB != null) {
                ordenDB.EstadoOrden = ordenEstado;
                ordenDB.EstadoPago = pagoEstado;
            }
        }

        public void ActualizarPagoStripeId(int id, string sessionId, string transaccionId)
        {
            var ordenDB = _dbContext.Ordenes.FirstOrDefault(o => o.Id == id);

            if (ordenDB != null)
            {
                if (!String.IsNullOrEmpty(sessionId))
                {
                    ordenDB.SessionId = sessionId;
                }

                if (!String.IsNullOrEmpty(transaccionId))
                {
                    ordenDB.TransaccionId = transaccionId;
                    ordenDB.FechaPago = DateTime.Now;
                }
            }
        }
    }
}
