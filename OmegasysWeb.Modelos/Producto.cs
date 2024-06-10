using OmegasysWeb.Modelos.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(60, ErrorMessage = CommunErrors.CaracteresMaximosSuperados60)]
        public string NumeroSerie { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(60, ErrorMessage = CommunErrors.CaracteresMaximosSuperados60)]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public double Precio { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public double Costo { get; set; }
        public string ImagenURL { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public bool Estado { get; set; }

        #region R E L A C I O N E S
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }
        public int? PadreId { get; set; }
        public virtual Producto Padre { get; set; }
        #endregion
    }
}
