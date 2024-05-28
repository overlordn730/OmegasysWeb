﻿using OmegasysWeb.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio.IRepositorio
{
    public interface IMarcaRepositorio: IRepositorio<Marca>
    {
        void actualizar(Marca marca);
    }
}
