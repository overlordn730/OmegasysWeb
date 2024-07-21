using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class InventarioDetalleRepositorio : Repositorio<InventarioDetalle>, IInventarioDetalleRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public InventarioDetalleRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(InventarioDetalle inventarioDetalle)
        {
            var inventarioDetalleDB = _dbContext.InventarioDetalles.FirstOrDefault(p => p.Id == inventarioDetalle.Id);
            if (inventarioDetalleDB != null)
            {
                inventarioDetalleDB.StockAnterior = inventarioDetalle.StockAnterior;
                inventarioDetalleDB.Cantidad = inventarioDetalle.Cantidad;

                _dbContext.SaveChanges();
            }
        }  
    }
}
