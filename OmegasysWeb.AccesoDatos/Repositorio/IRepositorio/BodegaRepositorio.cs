using OmegasysWeb.AccesoDatos.Data;
using OmegasysWeb.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio.IRepositorio
{
    public class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public BodegaRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public void actualizar(Bodega bodega)
        {
            var bodegaDB = _dbContext.Bodegas.FirstOrDefault(b => b.Id == bodega.Id);
            if (bodegaDB != null)
            {
                bodegaDB.Nombre = bodega.Nombre;
                bodegaDB.Descripcion = bodega.Descripcion;
                bodegaDB.Estado = bodega.Estado;
                _dbContext.SaveChanges();
            }
        }
    }
}
