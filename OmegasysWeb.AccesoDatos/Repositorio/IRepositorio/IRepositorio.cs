using OmegasysWeb.Modelos.Especificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task<T> obtener(int id);
        Task<IEnumerable<T>> obtenerTodos(
            Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );

        PagesList<T> ObtenerTodosPaginado(Parametros parametros, 
                                          Expression<Func<T,bool>> filtro = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          string incluirPropiedades = null,
                                          bool isTracking = true);

        Task<T> obtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string incluirPropiedades = null,
            bool isTracking = true);
        Task agregar( T entidad );
        void remover( T entidad );
        void removerRango( IEnumerable<T> entidad );
    }
}
