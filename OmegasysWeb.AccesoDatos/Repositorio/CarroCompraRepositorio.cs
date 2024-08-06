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
    public class CarroCompraRepositorio : Repositorio<CarroCompra>, ICarroCompraRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public CarroCompraRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(CarroCompra carroCompra)
        {
            _dbContext.Update(carroCompra);
        }
    }
}
