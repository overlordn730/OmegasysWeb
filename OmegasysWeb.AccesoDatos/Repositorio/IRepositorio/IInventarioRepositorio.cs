using Microsoft.AspNetCore.Mvc.Rendering;
using OmegasysWeb.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio.IRepositorio
{
    public interface IInventarioRepositorio: IRepositorio<Inventario>
    {
        void actualizar(Inventario inventario);

        IEnumerable<SelectListItem> obtenerTodosDropdownLista(string obj);
    }
}