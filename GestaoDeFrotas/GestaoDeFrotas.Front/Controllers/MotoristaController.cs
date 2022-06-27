﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Front.ViewModels;
using X.PagedList;
using GestaoDeFrotas.Business.BLL;
using GestaoDeFrotas.Business;
using GestaoDeFrotas.Shared.FiltroBusca;
using GestaoDeFrotas.Shared.Enums;

namespace GestaoDeFrotas.Controllers
{
    public class MotoristaController : Controller
    {
        #region Painel
        public ActionResult PainelDeMotoristas()
        {
            PainelMotoristasVM vm = new PainelMotoristasVM();
            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem(1, "Crescente"),
                    new DropDownItem(2, "Decrescente")
                };
            var OpcoesFiltragem = new List<DropDownItem>
                {
                    new DropDownItem(1, "Nome"),
                    new DropDownItem(2, "Data"),
                    new DropDownItem(3, "Status")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");
            vm.Motoristas = Enumerable.Empty<MotoristaVM>().ToPagedList(1, 15);

            try
            {
                //var FiltroBusca = new FiltroBusca
                //{
                //    ListaExpressao = new List<Expressao>
                //    {
                //        new Expressao()
                //        {
                //            OperadorLogico = EnumOperadorLogico.E,
                //            ListaCondicao = new List<Condicao>
                //            {
                //                new Condicao("DATAINCLUSAO", DateTime.Now.AddMonths(-6), EnumTipoCondicao.MAIOR_OU_IGUAL)
                //            }
                //        }
                //    }
                //};

                //var listaMotoristasDBE = new MotoristaDAL().Read(FiltroBusca).OrderBy(x => x.PrimeiroNome);
                var listaMotoristasDBE = new MotoristaDAL().List(false).OrderBy(x => x.PrimeiroNome);
                var listaMotoristasVM = new List<MotoristaVM>();

                vm.CastListaMotoristasParaVM(listaMotoristasDBE);

                vm.Motoristas = vm.Motoristas.ToPagedList(1, 15);
                vm.BuscaMotorista = "";
                vm.Todos = false;
                return View(vm);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel." + e.Message;
                return View(vm);
            }
        }
        public ActionResult BuscarMotoristasPainel(PainelMotoristasVM vm, int? pagina)
        {
            var numPagina = pagina ?? 1;
            if (vm.BuscaMotorista == null)
                vm.BuscaMotorista = string.Empty;
            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem(1, "Crescente"),
                    new DropDownItem(2, "Decrescente")
                };
            var OpcoesFiltragem = new List<DropDownItem>
                {
                    new DropDownItem(1, "Nome"),
                    new DropDownItem(2, "Data"),
                    new DropDownItem(3, "Status")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");
            try
            {
                // remove caracteres da string de busca
                vm.BuscaMotorista = StringTools.RemoverCaracteres(vm.BuscaMotorista, "-.");

                // busca por nome ou cpf
                var listaMotoristasDBE = new MotoristaDAL().List(vm.Todos)
                                                      .Where(m => (m.PrimeiroNome + " " + m.Sobrenome).ToUpper()
                                                      .Contains(vm.BuscaMotorista.ToUpper())
                                                      ||
                                                     (StringTools.RemoverCaracteres(m.CPF, "-."))
                                                     .Contains(vm.BuscaMotorista));

                var listaMotoristasVM = new List<MotoristaVM>();

                vm.CastListaMotoristasParaVM(listaMotoristasDBE);

                if (vm.OpcaoOrdenacao == "1")
                {
                    switch (vm.OpcoesFiltragem)
                    {
                        case "1":
                            vm.Motoristas = vm.Motoristas.OrderBy(m => m.PrimeiroNome);
                            break;
                        case "2":
                            vm.Motoristas = vm.Motoristas.OrderBy(m => m.DataInclusao);
                            break;
                        case "3":
                            vm.Motoristas = vm.Motoristas.OrderBy(m => m.Status);
                            break;
                        default:
                            vm.Motoristas = vm.Motoristas.OrderBy(m => m.PrimeiroNome);
                            break;
                    }
                }
                else
                {
                    switch (vm.OpcoesFiltragem)
                    {
                        case "1":
                            vm.Motoristas = vm.Motoristas.OrderByDescending(m => m.PrimeiroNome);
                            break;
                        case "2":
                            vm.Motoristas = vm.Motoristas.OrderByDescending(m => m.DataInclusao);
                            break;
                        case "3":
                            vm.Motoristas = vm.Motoristas.OrderByDescending(m => m.Status);
                            break;
                        default:
                            vm.Motoristas = vm.Motoristas.OrderByDescending(m => m.PrimeiroNome);
                            break;
                    }
                }

                vm.Motoristas = vm.Motoristas.ToPagedList(numPagina, 15);
                return View("PainelDeMotoristas", vm);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel.";
                vm.Motoristas = Enumerable.Empty<MotoristaVM>().ToPagedList(numPagina, 15);
                return View(vm);
            }
        }
        #endregion

        #region Incluir
        public ActionResult IncluirMotorista()
        {
            CadastroMotoristaVM vm = new CadastroMotoristaVM();

            try
            {
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<MunicipioDBE>(), "ID", "NomeMunicipio");
                ViewData["EstadoSelecionado"] = new SelectList(new EstadoDAL().List(), "ID", "NomeEstado");
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNHDAL().List(), "ID", "Categoria");

                return View(vm);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao preencher lista de dados! " + e.Message; ;

                return RedirectToAction("PainelDeMotoristas");
            }
        }

        [HttpPost]
        public ActionResult IncluirMotorista(CadastroMotoristaVM vm)
        {
            var Resposta = new Resposta<MotoristaDBE>();
            vm.CastToMotoristaVM();

            try
            {
                ViewData["EstadoSelecionado"] = new SelectList(new EstadoDAL().List(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["MunicipioSelecionado"] = new SelectList(new MunicipioDAL().ListarMunicipios(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNHDAL().List(), "ID", "Categoria", vm.CategoriaSelecionada);
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["EstadoSelecionado"] = new SelectList(Enumerable.Empty<MunicipioDBE>(), "ID", "NomeEstado");
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<EstadoDBE>(), "ID", "NomeMunicipio");
                ViewData["CategoriaSelecionada"] = new SelectList(Enumerable.Empty<CategoriaCNHDBE>(), "ID", "Categoria");
            }

            if (ModelState.IsValid)
            {
                try 
                {
                    new MotoristaBLL().CadastrarMotorista(vm.Motorista.CastToDBE(), ref Resposta);
                    switch (Resposta.Status)
                    {
                        case EnumStatusResposta.Erro:
                        case EnumStatusResposta.ErroValidacao:
                            TempData["MensagemErro"] = Resposta.Mensagem;
                            return View(vm);
                        case EnumStatusResposta.Aviso:
                            TempData["MensagemAviso"] = Resposta.Mensagem;
                            if(Resposta.Retorno.ID > 0)
                                return RedirectToAction("VisualizarMotorista", new { id = Resposta.Retorno.ID });
                            return View(vm);
                        default:
                            TempData["MensagemSucesso"] = Resposta.Mensagem;
                            return RedirectToAction("PainelDeMotoristas");
                    }
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = ex.Message;
                    return View(vm);
                }
            }

            return View(vm);
        }
        #endregion

        #region Editar
        public ActionResult VisualizarMotorista(int id)
        {
            MotoristaVM vm = new MotoristaVM();
            try
            {
                //vm.GetByID(id, true);
                vm.CastFromDBE(new MotoristaDAL().Read(id));
                vm.Endereco.Municipio = new MunicipioDAL().ListarMunicipios().Where(m => m.ID == vm.Endereco.Municipio.ID).First();
                vm.ListaVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(id, true);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do motorista - " + e.Message;
                return RedirectToAction("PainelDeMotoristas");
            }
            return View(vm);
        }

        public ActionResult EditarMotorista(int id)
        {
            CadastroMotoristaVM vm = new CadastroMotoristaVM();
            try
            {
                vm.Motorista.CastFromDBE(new MotoristaDAL().Read(id));
                vm.Motorista.ListaVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(id, true);
                vm.DataNascimento = vm.Motorista.DataNascimento.ToString("yyyy-MM-dd");
                vm.DataEmissao = vm.Motorista.CNH.DataEmissao.ToString("yyyy-MM-dd");
                vm.DataValidade = vm.Motorista.CNH.DataValidade.ToString("yyyy-MM-dd");
                ViewData["MunicipioSelecionado"] = new SelectList(new MunicipioDAL().ListarMunicipios(), "ID", "NomeMunicipio", vm.Motorista.Endereco.Municipio.ID.ToString());
                ViewData["EstadoSelecionado"] = new SelectList(new EstadoDAL().List(), "ID", "NomeEstado", vm.Motorista.Endereco.Municipio.Estado.ID.ToString());
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNHDAL().List(), "ID", "Categoria", vm.Motorista.CNH.Categoria.ID.ToString());
                return View(vm);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados de motorista e veículo! " + e.Message;
                return RedirectToAction("PainelDeMotoristas");
            }
        }

        [HttpPost]
        public ActionResult EditarMotorista(CadastroMotoristaVM vm)
        {
            vm.CastToMotoristaVM();
            var motoristaDBE = vm.Motorista.CastToDBE();
            try
            {
                vm.Motorista.ListaVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(vm.Motorista.ID, true);
                ViewData["MunicipioSelecionado"] = new SelectList(new MunicipioDAL().ListarMunicipios(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(new EstadoDAL().List(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNHDAL().List(), "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<MunicipioDBE>(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(Enumerable.Empty<EstadoDBE>(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(Enumerable.Empty<CategoriaCNHDBE>(), "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            if (ModelState.IsValid)
            {
                try
                {
                    new EnderecoDAL().Update(motoristaDBE.Endereco);
                    new CNHDAL().Update(motoristaDBE.CNH);
                    new MotoristaDAL().Update(motoristaDBE);

                    TempData["MensagemSucesso"] = "Cadastro editado com sucesso!";
                    return RedirectToAction("PainelDeMotoristas");
                }
                // Verificar inner exception
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message + " " + e.InnerException.Message;
                    return View(vm);
                }

            }
            return View(vm);
        }

        public ActionResult AlterarStatusMotorista(int id, bool status)
        {
            try
            {
                new MotoristaDAL().AtualizarStatus(id, !status);
                TempData["MensagemSucesso"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction("EditarMotorista", new { id });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = e.Message;
                return RedirectToAction("EditarMotorista", new { id });
            }
        }
        #endregion

        #region Vincular Veiculo
        [HttpPost]
        public ActionResult VincularVeiculo(CadastroMotoristaVM vm)
        {
            VeiculoDBE veiculo;
            if (vm.BuscaPlaca == null)
                vm.BuscaPlaca = string.Empty;
            try
            {
                vm.Motorista.CastFromDBE(new MotoristaDAL().Read(vm.Motorista.ID));
                vm.Motorista.ListaVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(vm.Motorista.ID, true);
                veiculo = new VeiculoDAL().BuscarPorPlaca(vm.BuscaPlaca.ToUpper(), true);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados de motorista e veículo - " + e.Message;
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }
            if (veiculo.ID == 0)
            {
                TempData["MensagemErro"] = "Veículo não encontrado ou inativo! Placa: " + vm.BuscaPlaca;
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }
            if (vm.Motorista.ListaVeiculos.Any(x => x.Placa == vm.BuscaPlaca.ToUpper()))
            {
                TempData["MensagemErro"] = "O Veículo informado já está vinculado ao motorista!";
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }
            try
            {
                new MotoristaDAL().VincularVeiculoMotorista(veiculo.ID, vm.Motorista.ID);
                TempData["MensagemSucesso"] = "Veículo vinculado com sucesso!";
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao vincular veículo - " + e.Message;
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }
        }
        public ActionResult DesvincularVeiculoMotorista(int veiculoid, int motoristaid)
        {
            try
            {
                new MotoristaDAL().DesvincularVeiculoMotorista(veiculoid, motoristaid);
                TempData["MensagemSucesso"] = "Veículo desvinculado com sucesso!";
                return RedirectToAction("EditarMotorista", new { id = motoristaid });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao desvincular veículo - " + e.Message;
                return RedirectToAction("EditarMotorista", new { id = motoristaid });
            }
        }
        #endregion

        #region Busca JSON
        [WebMethod]
        public JsonResult BuscaMunicipio(int idEstado)
        {
            IEnumerable<MunicipioDBE> ListaMunicipios = new MunicipioDAL().ListarMunicipios();
            return Json(new SelectList(ListaMunicipios
                .Where(x => x.Estado.ID == idEstado)
                .OrderBy(x => x.NomeMunicipio)
                , "ID", "NomeMunicipio"));
        }
        public ActionResult ValidaCpf(MotoristaDBE motorista)
        {
            var motoristaBusca = new MotoristaDAL().GetByCPF(motorista.CPF, false);

            if (motoristaBusca.ID > 0 && motoristaBusca.ID != motorista.ID)
                return Json("CPF já cadastrado", JsonRequestBehavior.AllowGet);

            if (!CPF.ValidaCpf(motorista.CPF))
            {
                return Json("CPF inválido", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }       
}