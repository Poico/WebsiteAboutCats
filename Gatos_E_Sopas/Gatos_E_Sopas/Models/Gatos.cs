using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Gatos_E_Sopas.Models
{
    public class Gatos
    {
        public int ID { get; set; }


        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }
        [Display(Name = "Raça")]
        public string Raca { get; set; }
        [Range(2, 20)]
        public int Idade { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Preço")]
        public double Preco { get; set; }
        public string Cor { get; set; }
        public string imagem_path { get; set; }
        public HttpPostedFileBase imagem { get; set; }
    }
}