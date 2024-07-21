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
    public class InventarioRepositorio : Repositorio<Inventario>, IInventarioRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public InventarioRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(Inventario inventario)
        {
            var inventarioDB = _dbContext.Inventarios.FirstOrDefault(p => p.Id == inventario.Id);
            if (inventarioDB != null)
            {
                inventarioDB.BodegaId = inventario.BodegaId;
                inventarioDB.FechaFinal = inventario.FechaFinal;
                inventarioDB.Estado = inventario.Estado;

                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> obtenerTodosDropdownLista(string obj)
        {
            if (obj == "Bodega")
            {
                return _dbContext.Bodegas
                    .Where(b => b.Estado == true)
                    .Select(b => new SelectListItem { Text = b.Nombre, Value = b.Id.ToString() });
            }
            return null;
        }
    }
}
