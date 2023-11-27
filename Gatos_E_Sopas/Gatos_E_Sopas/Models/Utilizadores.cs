using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Gatos_E_Sopas.Models
{
    public class Utilizadores
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Nome { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-Mail")]
        public string email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string pass { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma Password")]
        public string Confirmapass { get; set; }
    }
}