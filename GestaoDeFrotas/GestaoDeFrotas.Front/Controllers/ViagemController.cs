using CadastroDeCaminhoneiro.Enums;
using CadastroDeCaminhoneiro.Models;
using CadastroDeCaminhoneiro.DBEnums;
using CadastroDeCaminhoneiro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;

namespace CadastroDeCaminhoneiro.Controllers
{
    public class ViagemController : Controller
    {
        #region Painel
        public ActionResult PainelDeViagens()
        {
            PainelViagensVM vm = new PainelViagensVM();
            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                };
            var OpcoesFiltragem = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                };
            var OpcaoCampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.STATUS, "Status"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                };
            var StatusViagem = new List<DropDownItem>
                {
                    new DropDownItem(0, "Todos"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.PROGRAMADA, "Programada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.EMANDAMENTO, "Em Andamento"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.ENCERRADA, "Encerrada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.CANCELADA, "Cancelada")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcaoCampoOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");
            ViewData["StatusViagem"] = new SelectList(StatusViagem, "Id", "Text");

            vm.OpcoesFiltragem = 1;
            vm.OpcaoCampoOrdenacao = 2;
            vm.OpcaoOrdenacao = 2;

            vm.Viagens.ToPagedList(1, 10);

            try
            {
                vm.Viagens = ViagemHelper.BuscarViagensPainel(new ViagemDBE(), vm.BuscaViagem, vm.OpcoesFiltragem, vm.OpcaoCampoOrdenacao, vm.OpcaoOrdenacao).ToPagedList(1, 10);
            }
            catch(Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel: " + e.Message;
                return View(vm);
            }
            return View(vm);
        }

        public ActionResult BuscarViagensPainel(PainelViagensVM vm, int? pagina)
        {
            var numPagina = pagina ?? 1;

            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                };
            var OpcoesFiltragem = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                };
            var OpcaoCampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.STATUS, "Status"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                };
            var StatusViagem = new List<DropDownItem>
                {
                    new DropDownItem(0, "Todos"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.PROGRAMADA, "Programada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.EMANDAMENTO, "Em Andamento"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.ENCERRADA, "Encerrada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.CANCELADA, "Cancelada")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcaoCampoOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");
            ViewData["StatusViagem"] = new SelectList(StatusViagem, "Id", "Text");

            vm.Viagens = Enumerable.Empty<ViagemDBE>().ToPagedList(numPagina, 10);

            try
            {
                var ViagemBusca = new ViagemDBE();
                ViagemBusca.ViagemStatus.ID = vm.StatusViagem;

                vm.Viagens = ViagemHelper.BuscarViagensPainel(ViagemBusca, vm.BuscaViagem, vm.OpcoesFiltragem, vm.OpcaoCampoOrdenacao, vm.OpcaoOrdenacao);

                vm.Viagens = vm.Viagens.ToPagedList(numPagina, 10);

                return View("PainelDeViagens", vm);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel. " + ex.Message;
                
                return View("PainelDeViagens", vm);
            }
        }
        #endregion

        #region Cadastro
        public ActionResult VisualizarViagem(int id)
        {
            ViagemDBE viagem;
            try
            {
                viagem = new ViagemDAL().Read(id);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados da viagem: " + e.Message;
                return RedirectToAction("PainelDeViagens");
            }
            return View(viagem);
        }

        public ActionResult IncluirViagem()
        {
            CadastroViagemVM vm = new CadastroViagemVM();
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<VeiculoDBE>(), "ID", "Placa");
            try
            {
                ViewData["MotivoID"] = new SelectList(new MotivoViagemDAL().List(), "ID", "Descricao");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao preencher lista de dados! " + e.Message; ;
                return RedirectToAction("PainelDeViagens");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirViagem(CadastroViagemVM vm)
        {
            ViewData["MotivoID"] = new SelectList(Enumerable.Empty<MotivoViagemDBE>(), "ID", "Descricao");
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<VeiculoDBE>(), "ID", "Placa");

            try
            {
                ViewData["MotivoID"] = new SelectList(new MotivoViagemDAL().List(), "ID", "Descricao");
                ViewData["VeiculoID"] = new SelectList(new VeiculoDAL().ListarVeiculosPorIDMotorista(vm.MotoristaID, true), "ID", "Placa", vm.VeiculoID);

                vm.VmToDBE();

                GeradorCodigoViagem.GerarCodigo(vm.Viagem);

                vm.Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.PROGRAMADA;

                var retorno = ViagemValidador.ValidaInclusao(vm.Viagem);
                if (retorno.Sucesso)
                {
                    new ViagemDAL().Create(vm.Viagem);
                    //vm.Viagem.Create();
                    TempData["MensagemSucesso"] = "Viagem cadastrada com sucesso!";
                    return RedirectToAction("PainelDeViagens");
                }

                TempData["MensagemAviso"] = retorno.Mensagem;
                return View(vm);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao cadastrar viagem!" + e.Message;
                return View(vm);
            }
        }

        public ActionResult AdministrarViagem(int id)
        {
            CadastroViagemVM vm = new CadastroViagemVM();
            try
            {
                vm.Viagem = new ViagemDAL().Read(id);
                vm.Viagem.MotoristaViagem = new MotoristaDAL().Read(vm.Viagem.MotoristaViagem.ID);
                vm.Viagem.VeiculoViagem = new VeiculoDAL().Read(vm.Viagem.VeiculoViagem.ID);
                vm.DBEToVM();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados da viagem: " + e.Message;
                return RedirectToAction("PainelVigens");
            }
            return View(vm);
        }

        public ActionResult IniciarViagem(int id)
        {
            ViagemDBE Viagem;
            try
            {
                Viagem = new ViagemDAL().Read(id);
                Viagem.Inicio = DateTime.Now;
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.EMANDAMENTO;
                new ViagemDAL().Update(Viagem);

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " iniciada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao iniciar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id });
            }
        }
        public ActionResult FinalizarViagem(int id)
        {
            ViagemDBE Viagem;
            try
            {
                Viagem = new ViagemDAL().Read(id);
                Viagem.Fim = DateTime.Now;
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.ENCERRADA;
                new ViagemDAL().Update(Viagem);

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " encerrada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao encerrar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id });
            }
        }
        public ActionResult CancelarViagem(int id)
        {
            ViagemDBE Viagem;
            try
            {
                Viagem = new ViagemDAL().Read(id);
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.CANCELADA;
                new ViagemDAL().Update(Viagem);

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " cancelada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao cancelar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id });
            }
        }
        #endregion

        #region Buscas Json
        public JsonResult BuscaMotorista(string busca)
        {
            var lista = MotoristaHelper.BuscarPorNomeOuCPF(busca, false).OrderByDescending(m => m.ID);

            return Json(lista.First());
        }

        public JsonResult BuscaVeiculosPorMotoristaID(string motoristaID)
        {
            var veiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(Convert.ToInt32(motoristaID), true);
            return Json(new SelectList(veiculos, "ID", "Placa"));
        }

        public JsonResult SelecionaVeiculo(string veiculoID)
        {
            var veiculo = new VeiculoDAL().BuscarPorId(Convert.ToInt32(veiculoID), true);
            return Json(veiculo);
        }

        [HttpPost]
        public JsonResult AutoCompleteMotorista(string busca)
        {
            var lista = new List<AutoCompleteMotorista>();
            foreach (var item in MotoristaHelper.BuscarPorNomeOuCPF(busca, false).OrderByDescending(m => m.ID))
            {
                lista.Add(new AutoCompleteMotorista(item));
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
