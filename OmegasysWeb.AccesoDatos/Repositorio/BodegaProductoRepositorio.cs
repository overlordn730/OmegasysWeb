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
    public class BodegaProductoRepositorio : Repositorio<BodegaProducto>, IBodegaProductoRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public BodegaProductoRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(BodegaProducto bodegaProducto)
        {
            var bodegaProductoDB = _dbContext.BodegasProductos.FirstOrDefault(p => p.Id == bodegaProducto.Id);
            if (bodegaProductoDB != null)
            {
                bodegaProductoDB.Cantidad = bodegaProducto.Cantidad;

                _dbContext.SaveChanges();
            }
        }  
    }
}
