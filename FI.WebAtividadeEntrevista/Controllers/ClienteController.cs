using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if(ValidaCPF(model.CPF) == true){

                    string valor = model.CPF.Replace(".", "").Replace("-", "");

                    if (bo.VerificarExistencia(valor) == false) {
                        model.Id = bo.Incluir(new Cliente()
                        {
                            CPF = model.CPF,
                            CEP = valor,
                            Cidade = model.Cidade,
                            Email = model.Email,
                            Estado = model.Estado,
                            Logradouro = model.Logradouro,
                            Nacionalidade = model.Nacionalidade,
                            Nome = model.Nome,
                            Sobrenome = model.Sobrenome,
                            Telefone = model.Telefone
                        }); ;
                        return Json("Cadastro efetuado com sucesso");
                    }
                    else
                    {
                        return Json("Cpf já cadastrado! \n Por favor verifique o CPF digitado." );
                    }

                }else{
                    return Json("Cpf inválido! \n Por favor verifique o CPF digitado.");
                }
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (ValidaCPF(model.CPF) == true)
                {
                    string valor = model.CPF.Replace(".", "").Replace("-", "");

                    if (bo.VerificarExistencia(valor) == false)
                    {
                        bo.Alterar(new Cliente()
                        {
                            Id = model.Id,
                            CPF = valor,
                            CEP = model.CEP,
                            Cidade = model.Cidade,
                            Email = model.Email,
                            Estado = model.Estado,
                            Logradouro = model.Logradouro,
                            Nacionalidade = model.Nacionalidade,
                            Nome = model.Nome,
                            Sobrenome = model.Sobrenome,
                            Telefone = model.Telefone
                        });
                    return Json("Cadastro alterado com sucesso");
                }
                else
                {
                    return Json("Cpf já cadastrado! \n Por favor verifique o CPF digitado.");
                }

            }else{
                    return Json("Cpf inválido! \n Por favor verifique o CPF digitado.");
                }
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {

                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CPF = Convert.ToUInt64(cliente.CPF).ToString(@"000\.000\.000\-00"),
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone
                };

            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public static bool ValidaCPF(string vrCPF)
        {
            string valor = vrCPF.Replace(".", "").Replace("-", "");

            if (valor.Length != 11) {
                return false;
            }

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
            {
                if (valor[i] != valor[0]) { 
                igual = false;
                }

                if (igual || valor == "12345678909") { 
                    return false;
                } 
            }

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
            {
                numeros[i] = int.Parse(
                  valor[i].ToString());
            }

            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (10 - i) * numeros[i];
            }

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                {
                    return false;
                }
            }
            else if (numeros[9] != 11 - resultado)
            {
                return false;
            }

            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (11 - i) * numeros[i];
            }

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                {
                    return false;
                }
            }
            else if (numeros[10] != 11 - resultado)
            {
                return false;
            }

            return true;

        }

    }
}