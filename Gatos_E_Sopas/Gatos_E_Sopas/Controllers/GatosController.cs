using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gatos_E_Sopas.Models;
using System.Data;
using System.IO;
using System.Data.SqlClient;


namespace Gatos_E_Sopas.Controllers
{
    public class GatosController : Controller
    {
        BaseDados bd = new BaseDados();
        // GET: Gatos
        public ActionResult InserirGato()
        {
            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");
            List<Gatos> gatos = new List<Gatos>();
            return View();
        }

        [HttpPost]
        public ActionResult InserirGato(Gatos gato)
        {
            if (ModelState.IsValid)
            {
                //Guardar dados do gato na Base de Dados

                string sqltext = "Insert into Gatos(Nome,Raca,Idade,Descricao,Preco,Cor)";
                sqltext += " Values(@Nome,@Raca,@Idade,@Descricao,@Preco,@Cor)";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Nome", gato.Nome));
                parametros.Add(new SqlParameter("@Raca", gato.Raca));
                parametros.Add(new SqlParameter("@Idade", gato.Idade));
                parametros.Add(new SqlParameter("@Descricao", gato.Descricao));
                parametros.Add(new SqlParameter("@Preco", gato.Preco));
                parametros.Add(new SqlParameter("@Cor", gato.Cor));
                bd.executa_SQL(sqltext, parametros);

                if (gato.imagem!=null)
                {
                    // Ir buscar a extensão
                    string extensaoImagem = Path.GetExtension(gato.imagem.FileName);
                    // ir ver o último ID da tabela que foi adicionado posteriormente
                    string ID = bd.devolve_consulta("Select Max(ID) From Gatos").Rows[0][0].ToString();
                    //Dar update ao último registo da tabela com o caminho da imagem
                    bd.executa_SQL("Update Gatos Set imagem_path='" + ID + extensaoImagem + "' Where ID=" + ID);
                    // Copiar imagem para uma pasta 
                    gato.imagem.SaveAs(ControllerContext.HttpContext.Server.MapPath("\\Content\\imagens\\" + ID + extensaoImagem));

                }
                


                return RedirectToAction("ListaGatos/0");
            }
            return View();
        }
        public ActionResult editar(string id)
        {

            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");

            // Carregar os dados do gato escolhido da Base de Dados
            DataTable dados_gatos = bd.devolve_consulta("Select * from Gatos where ID='" + id + "'");


            // Carregar os dados para o objeto
            Gatos gato = new Gatos();
            gato.ID = Convert.ToInt32(dados_gatos.Rows[0][0]);
            gato.Nome = dados_gatos.Rows[0][1].ToString();
            gato.Raca = dados_gatos.Rows[0][2].ToString();
            gato.Idade = Convert.ToInt32(dados_gatos.Rows[0][3]);
            gato.Descricao = dados_gatos.Rows[0][4].ToString();
            gato.Preco = Convert.ToDouble(dados_gatos.Rows[0][5]);
            gato.Cor = dados_gatos.Rows[0][6].ToString();
            gato.imagem_path = dados_gatos.Rows[0][7].ToString();

            return View(gato);
        }
        [HttpPost]
        public ActionResult editar(Gatos gato)
        {
            // Script para editar os dados da tabela
            string sqltext = "Update Gatos set Nome=@Nome, Raca=@Raca, Idade=@Idade, Descricao=@Descricao, Preco=@Preco, Cor=@Cor, where ID='" + gato.ID + "'";
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Nome", gato.Nome));
            parametros.Add(new SqlParameter("@Raca", gato.Raca));
            parametros.Add(new SqlParameter("@Idade", gato.Idade));
            parametros.Add(new SqlParameter("@Descricao", gato.Descricao));
            parametros.Add(new SqlParameter("@Preco", gato.Preco));
            parametros.Add(new SqlParameter("@Cor", gato.Cor));
            bd.executa_SQL(sqltext, parametros);

            return RedirectToAction("ListaGatos/0");
        }


        public ActionResult listagatos(int id)
        {
            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");

            List<Gatos> gatos = new List<Gatos>();
            //Query para listar todos os gatos
            DataTable dados_gatos = bd.devolve_consulta("Select * from Gatos");

            // script para saber qnts páginas vai ser preciso
            int ultimo_gato = id * 5 + 5;
            if (ultimo_gato > dados_gatos.Rows.Count)
                ultimo_gato = dados_gatos.Rows.Count;

            for (int i = id * 5; i < ultimo_gato; i++)
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

            ViewBag.npagina = id;
            ViewBag.ntotalpaginas = dados_gatos.Rows.Count / 5;
            return View(gatos);
        }

        public ActionResult detalhes(string id)
        {
            Gatos gato = new Gatos();
            DataTable dados_gatos = bd.devolve_consulta("Select * from Gatos where ID='" + id + "'");

            //Apresentar os detalhes do gato
            gato.ID = Convert.ToInt32(dados_gatos.Rows[0][0]);
            gato.Nome = dados_gatos.Rows[0][1].ToString();
            gato.Raca = dados_gatos.Rows[0][2].ToString();
            gato.Idade = Convert.ToInt32(dados_gatos.Rows[0][3]);
            gato.Descricao = dados_gatos.Rows[0][4].ToString();
            gato.Preco = Convert.ToDouble(dados_gatos.Rows[0][5]);
            gato.Cor = dados_gatos.Rows[0][6].ToString();
            gato.imagem_path = dados_gatos.Rows[0][7].ToString();

            return View(gato);
        }
        public ActionResult Apagar(string id)
        {
            
            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");

            Gatos gato = new Gatos();
            //consulta para ir buscar o gato com o ID igual
            DataTable dados_gatos = bd.devolve_consulta("Select * from Gatos where ID='" + id + "'");

            gato.ID = Convert.ToInt32(dados_gatos.Rows[0][0]);
            gato.Nome = dados_gatos.Rows[0][1].ToString();
            gato.Raca = dados_gatos.Rows[0][2].ToString();
            gato.Idade = Convert.ToInt32(dados_gatos.Rows[0][3]);
            gato.Descricao = dados_gatos.Rows[0][4].ToString();
            gato.Preco = Convert.ToDouble(dados_gatos.Rows[0][5]);
            gato.Cor = dados_gatos.Rows[0][6].ToString();
            gato.imagem_path = dados_gatos.Rows[0][7].ToString();

            //Inserir o ID para dps se a pessoa quiser mesmo apagar saber o ID 
            TempData["ID"] = gato.ID;

            return View(gato);
        }
        [HttpPost]
        public ActionResult apagar(Gatos gato)
        {
            // query para apagar com o ID selecionado
            bd.executa_SQL("Delete from Gatos where ID='" + TempData["ID"] + "'");
            return RedirectToAction("listagatos/0");
        }

        public ActionResult Pesquisar_Raca()
        {
            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");

            List<Gatos> gatos = new List<Gatos>();

            //Inserir os valores na dropdown
            DataTable racas = bd.devolve_consulta("select distinct raca from Gatos");
            string[] lista_racas = new string[racas.Rows.Count];
            for (int i = 0; i < racas.Rows.Count; i++)
            {
                lista_racas[i] = racas.Rows[i][0].ToString();
            }

            ViewData["raca"] = lista_racas;
            return View(gatos);
        }

        [HttpPost]
        [ActionName("Pesquisar_Raca")]

        public ActionResult Pesquisar_Raca_Resultado()
        {
            // Query sql para ir buscar o ddl_raca que é uma dropdown list dinamica e ver as raça igual aos gatos da db
            DataTable dados_gatos = bd.devolve_consulta("Select * from Gatos where raca = '" + Request.Form["ddl_raca"].ToString() + "'");
            List<Gatos> gatos = new List<Gatos>();

            //Inserir os dados na vista a partir da query
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

        public ActionResult Pesquisar_Nome()
        {
            if (Session["perfil"] == null)
                return RedirectToAction("Login", "utilizadores");
            List<Gatos> gatos = new List<Gatos>();
            return View(gatos);
        }

        [HttpPost]
        [ActionName("Pesquisar_Nome")]

        public ActionResult Pesquisar_Nome_Resultado() // Pesquisar pelo nome do gato
        {
            // Query sql para ir buscar o Txt_Nome da vista faz pesquisa com o like para tentar achar parecidos
            DataTable dados_gatos = bd.devolve_consulta("Select * from gatos where Nome like '%" + Request.Form["txt_Nome"] + "%'");
            List<Gatos> gatos = new List<Gatos>();

            //Inserir os dados na vista a partir da query
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
        public ActionResult compras(string id)
        {
            if (Session["perfil"] != null)
            {
                    
            }
            else
                return RedirectToAction("login", "utilizador");

            Gatos gato = new Gatos();
            DataTable dados_gatos = bd.devolve_consulta("select * from gatos where ID='" + id + "'");
            gato.ID = Convert.ToInt32(dados_gatos.Rows[0][0]);
            gato.Nome = dados_gatos.Rows[0][1].ToString();
            gato.Raca = dados_gatos.Rows[0][2].ToString();
            gato.Idade = Convert.ToInt32(dados_gatos.Rows[0][3]);
            gato.Descricao = dados_gatos.Rows[0][4].ToString();
            gato.Preco = Convert.ToDouble(dados_gatos.Rows[0][5]);
            gato.Cor = dados_gatos.Rows[0][6].ToString();
            gato.imagem_path = dados_gatos.Rows[0][7].ToString();

            Session["IDGato"] = gato.ID;

            return View(gato);
        }

        [HttpPost]
        public ActionResult compras(Gatos gatos)
        {
            string sim = bd.devolve_consulta("Select ID From Utilizador where Nome='"+Session["Nome"].ToString()+"'").Rows[0][0].ToString();
            string nao = Session["IDGato"].ToString();
            if (bd.devolve_consulta("Select count(ID) From Compras where GatosID=" + Session["IDGato"]).Rows[0][0].ToString()=="0")
            {
                bd.executa_SQL("Insert into Compras(GatosID, UtilizadorID,estado) values('" + nao + "','" + sim + "'," + "'Reservado'" + ")");
                ViewBag.mensagem = "O seu gato foi reservado";
            }
            else
            {
                ViewBag.mensagem = "Gato já reservado";
                return View(gatos);
            }
            return RedirectToAction("listagatos/0");

        }

        public ActionResult novidades()
        {
            DataTable dados_gatos = bd.devolve_consulta("Select top 3 Nome, imagem_path, raca,ID from Gatos order by Idade desc");
            List<Gatos> gatos = new List<Gatos>();
            for (int i = 0; i < dados_gatos.Rows.Count; i++)
            {
                gatos.Add(new Gatos()
                {
                    Nome = dados_gatos.Rows[i][0].ToString(),
                    imagem_path = dados_gatos.Rows[i][1].ToString(),
                    Raca = dados_gatos.Rows[i][2].ToString(),
                    ID = Convert.ToInt32(dados_gatos.Rows[i][3]),
                });
            }

            return View(gatos);
        }

        public ActionResult lista_compras()
        {
            if (Session["perfil"] == null)
                return RedirectToAction("login", "utilizador");

            DataTable dados_gatos;

            dados_gatos = bd.devolve_consulta("Select *, Gatos.Nome from compras, Gatos where gatos.ID = compras.gatosID and UtilizadorID ='"+Session["ID"].ToString()+"'");
            string sim = Session["ID"].ToString();
            List<Gatos> gatos = new List<Gatos>();

            for (int i = 0; i < dados_gatos.Rows.Count; i++)
            {
                gatos.Add(new Gatos()
                {
                    Nome = dados_gatos.Rows[0][5].ToString(),
                    Raca = dados_gatos.Rows[0][6].ToString(),
                    Preco = Convert.ToDouble(dados_gatos.Rows[0][9]),
                });
            }

            return View(gatos);
        }
    }
}