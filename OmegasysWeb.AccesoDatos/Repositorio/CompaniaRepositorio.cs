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
    public class CompaniaRepositorio : Repositorio<Compania>, ICompaniaRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public CompaniaRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(Compania compania)
        {
            var companiaDB = _dbContext.Companias.FirstOrDefault(b => b.Id == compania.Id);
            if (companiaDB != null)
            {
                companiaDB.Nombre = compania.Nombre;
                companiaDB.Descripcion = compania.Descripcion;
                companiaDB.Pais = compania.Pais;
                companiaDB.Ciudad = compania.Ciudad;
                companiaDB.Direccion = compania.Direccion;
                companiaDB.Telefono = compania.Telefono;
                companiaDB.BodegaVentaId = compania.BodegaVentaId;
                companiaDB.ActualizadoPorId = compania.ActualizadoPorId;
                companiaDB.FechaActualizacion = compania.FechaActualizacion;

                _dbContext.SaveChanges();
            }
        }
    }
}
