using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gatos_E_Sopas.Models
{
    public class Compras
    {
        public int ID { get; set; }
        public int idGato{ get; set; }
        public int ídUtilizador { get; set; }
        public string estado { get; set; }
    }
}