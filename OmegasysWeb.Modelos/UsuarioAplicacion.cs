using Microsoft.AspNetCore.Identity;
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
    public class UsuarioAplicacion: IdentityUser
    {
        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(80)]
        public string Nombres { get; set; }

        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(80)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(200)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(200)]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = CommunErrors.CampoObligatorio)]
        [MaxLength(200)]
        public string Pais { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
