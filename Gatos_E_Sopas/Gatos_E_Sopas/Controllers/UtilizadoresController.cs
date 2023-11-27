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
    public class UtilizadoresController : Controller
    {
        BaseDados bd = new BaseDados();

        // GET: Utilizador
        public ActionResult Registo()
        {
            return View();
        }

        // Post - Guardar os dados na Base de Dados
        [HttpPost]
        public ActionResult Registo(Utilizadores user)
        {
            // Verificar se existe um utilizador com o Nome
            if (bd.devolve_consulta("select count(nome) from Utilizador where Nome='" + user.Nome.ToString() + "'").Rows[0][0].ToString() == "1")
            {
                ViewBag.erroutilizador = "Já existe um utilizador com esse Nome";
                return View();
            }

            // Testar se a password é igual ao confirmar
            if (user.pass != user.Confirmapass)
            {
                ViewBag.erropassword = "Password's não coincidem";
                return View();
            }

            // Testar se a password tem 8 caracteres
            if (user.pass.Length < 8)
            {
                ViewBag.erropassword = "A password tem menos de 8 caracteres";
                return View();
            }

            // Guardar os dados na tabela da BD
            string sql = "insert into utilizador (Nome, Password, email, Admin) values (@Nome, HASHBYTES('MD5',@Password), @email, 0)";

            List<SqlParameter> list = new List<SqlParameter>
            {
                new SqlParameter(){ParameterName="@Nome", SqlDbType=SqlDbType.NVarChar, Value=user.Nome},
                new SqlParameter(){ParameterName="@Password", SqlDbType=SqlDbType.NVarChar, Value=user.pass},
                new SqlParameter(){ParameterName="@email", SqlDbType=SqlDbType.NVarChar, Value=user.email},
            };

            bd.executa_SQL(sql, list);
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Utilizadores user)
        {
            // Testar se o login está correto
            if (bd.devolve_consulta("select count(Nome) from utilizador where Nome='" + user.Nome + "'").Rows[0][0].ToString() == "0")
            {
                ViewBag.erroNome = "O utilizador não existe";
                return View();
            }
            else
            {
                string sql = "select count(Nome) from utilizador where Password = HASHBYTES('MD5',@Password)";
                List<SqlParameter> list = new List<SqlParameter>
                {
                    new SqlParameter(){ParameterName="@Password", SqlDbType=SqlDbType.NVarChar, Value=user.pass},
                };

                if (bd.devolve_consulta(sql, list).Rows[0][0].ToString() == "0")
                {
                    ViewBag.erropassword = "Password Incorreta";
                    return View();
                }
                else
                {
                    Session["Nome"] = user.Nome;
                    Session["ID"] = bd.devolve_consulta("select ID from utilizador where Nome='" + Session["Nome"] + "'").Rows[0][0].ToString();
                    string nao = "Select admin from Utilizador where Utilizador.Nome='" + user.Nome + "'";
                    string sim = bd.devolve_consulta(nao).Rows[0][0].ToString();
                        Session["perfil"] = sim;
                    return RedirectToAction("index", "home");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("index", "home");
        }

        public ActionResult perfil()
        {
            if (Session["perfil"] == null)
                return RedirectToAction("login", "utilizador");

            DataTable dados = bd.devolve_consulta("Select * from utilizador where Nome='" + Session["Nome"] + "'");
            Utilizadores user = new Utilizadores();
            user.email = dados.Rows[0][2].ToString();
            return View(user);
        }

        [HttpPost]
        public ActionResult perfil(Utilizadores user)
        {
            if (user.pass != user.Confirmapass)
            {
                ViewBag.error = "As password's não coincidem";
                return View();
            }
            else if (user.pass.Length < 8)
            {
                ViewBag.erropassword = "A password tem de ter 8 ou mais caracteres";
                return View();
            }
            else
            {
                string sql = "update utilizador set email = @email, Password = HASHBYTES('MD5', @Password) where Nome ='" + Session["Nome"] + "'";
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Password", user.pass));
                parametros.Add(new SqlParameter("@email", user.email));
                bd.executa_SQL(sql, parametros);
                return RedirectToAction("index", "home");
            }
        }
    }
}