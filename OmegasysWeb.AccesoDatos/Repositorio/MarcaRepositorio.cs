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
    public class MarcaRepositorio: Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public MarcaRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(Marca marca)
        {
            var marcaDB = _dbContext.Marcas.FirstOrDefault(b => b.Id == marca.Id);
            if (marcaDB != null)
            {
                marcaDB.Nombre = marca.Nombre;
                marcaDB.Descripcion = marca.Descripcion;
                marcaDB.Estado = marca.Estado;
                _dbContext.SaveChanges();
            }
        }
    }
}
