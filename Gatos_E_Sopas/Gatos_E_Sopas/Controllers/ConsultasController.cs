using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gatos_E_Sopas.Models;
using System.Data;

namespace Gatos_E_Sopas.Controllers
{
    public class ConsultasController : Controller
    {
        BaseDados bd = new BaseDados();
        // GET: Gatos
        public ActionResult pesquisa_nome()
        {
            List<Gatos> gatos = new List<Gatos>();
            return View();
        }
        [HttpPost]
        [ActionName("Pesquisar Nome dos Gatos")]
        public ActionResult pesquisa_nome_resultado()
        {
            DataTable dados_gatos = bd.devolve_consulta("Select top 3 * From Gatos where Nome like '%" + Request.Form["txt_pesquisa"] + "%'");
            List<Gatos> gatos = new List<Gatos>();
            for (int i = 0; i < dados_gatos.Rows.Count; i++)
            {
                gatos.Add(new Gatos()
                {
                    ID = Convert.ToInt32(dados_gatos.Rows[i][0]),
                    Nome = dados_gatos.Rows[i][1].ToString(),
                    Raca = dados_gatos.Rows[i][2].ToString(),
                    Idade = Convert.ToInt32(dados_gatos.Rows[i][3]),
                    Descricao = dados_gatos.Rows[i][4].ToString(),
                    Preco = Convert.ToDouble(dados_gatos.Rows[i][5]),
                    Cor = dados_gatos.Rows[i][6].ToString(),
                    imagem_path = dados_gatos.Rows[i][7].ToString(),
                });
            }
            return View(gatos);
        }
        public ActionResult pesquisa_Raca()
        {
            List<Gatos> gatos = new List<Gatos>();
            DataTable dados_gatos = bd.devolve_consulta("Select distinct Raca From Gatos");
            string[] Racas = new string[dados_gatos.Rows.Count];
            for (int i = 0; i < dados_gatos.Rows.Count; i++)
            {
                Racas[i] = dados_gatos.Rows[i][0].ToString();
            }
            ViewData["autores"] = Racas;
            return View(gatos);
        }

        [HttpPost]
        [ActionName("pesquisa_Raca")]
        public ActionResult pesquisa_Raca_resultado()
        {
            DataTable dados_gatos = bd.devolve_consulta("Select * From Gatos where Raca like '%" + Request.Form["ddl_Raca"] + "%'");
            List<Gatos> gatos = new List<Gatos>();

            for (int i = 0; i < dados_gatos.Rows.Count; i++)
            {
                gatos.Add(new Gatos()
                {
                    ID = Convert.ToInt32(dados_gatos.Rows[i][0]),
                    Nome = dados_gatos.Rows[i][1].ToString(),
                    Raca = dados_gatos.Rows[i][2].ToString(),
                    Idade = Convert.ToInt32(dados_gatos.Rows[i][3]),
                    Descricao = dados_gatos.Rows[i][4].ToString(),
                    Preco = Convert.ToDouble(dados_gatos.Rows[i][5]),
                    Cor = dados_gatos.Rows[i][6].ToString(),
                    imagem_path = dados_gatos.Rows[i][7].ToString(),
                });
            }
            return View(gatos);
        }
    }
}