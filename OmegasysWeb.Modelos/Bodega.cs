using OmegasysWeb.Modelos.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasysWeb.Modelos
{
    public class Bodega
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(60, ErrorMessage = CommunErrors.CaracteresMaximosSuperados60)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(100, ErrorMessage = CommunErrors.CaracteresMaximosSuperados100)]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        public bool Estado { get; set; }
    }
}
