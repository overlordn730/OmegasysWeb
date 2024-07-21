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
        public ICategoriaRepositorio Categoria {  get; private set; }
        public IMarcaRepositorio Marca { get; private set; }
        public IProductoRepositorio Producto { get; private set; }
        public IUsuarioAplicacionRepositorio UsuarioAplicacion { get; private set; }
        public IBodegaProductoRepositorio BodegaProducto { get; private set; }
        public IInventarioRepositorio Inventario { get; private set; }
        public IInventarioDetalleRepositorio InventarioDetalle { get; private set; }
        public IKardexInventarioRepositorio KardexInventario { get; private set; }

        public UnidadTrabajo(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            Bodega = new BodegaRepositorio(_dbContext);
            Categoria = new CategoriaRepositorio(_dbContext);
            Marca = new MarcaRepositorio(_dbContext);
            Producto = new ProductoRepositorio(_dbContext);
            UsuarioAplicacion = new UsuarioAplicacionRepositorio(_dbContext);
            BodegaProducto = new BodegaProductoRepositorio(dbContext);
            Inventario = new InventarioRepositorio(dbContext);
            InventarioDetalle = new InventarioDetalleRepositorio(dbContext);
            KardexInventario = new KardexInventarioRepositorio(dbContext);
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
