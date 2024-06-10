using Microsoft.AspNetCore.Mvc.Rendering;
using OmegasysWeb.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio.IRepositorio
{
    public interface IProductoRepositorio: IRepositorio<Producto>
    {
        void actualizar(Producto producto);

        IEnumerable<SelectListItem> obtenerTodosDropdownList(string objeto);
    }
}
