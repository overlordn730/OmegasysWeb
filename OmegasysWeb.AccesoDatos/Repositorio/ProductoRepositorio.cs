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
    public class ProductoRepositorio: Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductoRepositorio(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void actualizar(Producto producto)
        {
            var productoDB = _dbContext.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (productoDB != null)
            {
                if(producto.ImagenURL != null)
                {
                    productoDB.ImagenURL = producto.ImagenURL;
                }
                productoDB.NumeroSerie = producto.NumeroSerie;
                productoDB.Descripcion = producto.Descripcion;
                productoDB.Precio = producto.Precio;
                productoDB.Costo = producto.Costo;
                productoDB.CategoriaId = producto.CategoriaId;
                productoDB.MarcaId = producto.MarcaId;
                productoDB.PadreId = producto.PadreId;
                productoDB.Estado = producto.Estado;

                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> obtenerTodosDropdownList(string objeto)
        {
            if(objeto == "Categoria")
            {
                return _dbContext.Categorias.Where(x => x.Estado == true).Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
            }

            if (objeto == "Marca")
            {
                return _dbContext.Marcas.Where(x => x.Estado == true).Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
            }
            
            if (objeto == "Producto")
            {
                return _dbContext.Productos.Where(x => x.Estado == true).Select(x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.Id.ToString()
                });
            }

            return null;
        }
    }
}
