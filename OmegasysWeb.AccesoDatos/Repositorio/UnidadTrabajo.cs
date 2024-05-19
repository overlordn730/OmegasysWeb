using OmegasysWeb.AccesoDatos.Data;
using OmegasysWeb.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _dbContext;
        public IBodegaRepositorio Bodega {  get; private set; }

        public UnidadTrabajo(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            Bodega = new BodegaRepositorio(_dbContext);

        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task Guardar()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
