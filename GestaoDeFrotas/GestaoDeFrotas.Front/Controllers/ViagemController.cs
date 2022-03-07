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
                vm.Viagens = ViagemHelper.BuscarViagensPainel(new Viagem(), vm.BuscaViagem, vm.OpcoesFiltragem, vm.OpcaoCampoOrdenacao, vm.OpcaoOrdenacao).ToPagedList(1, 10);
            }
            catch(Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel.";
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

            vm.Viagens = Enumerable.Empty<Viagem>().ToPagedList(numPagina, 10);

            try
            {
                var ViagemBusca = new Viagem();
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
            Viagem viagem = new Viagem();
            try
            {
                viagem.Read(id);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados da viagem.";
                return RedirectToAction("PainelDeViagens");
            }
            return View(viagem);
        }

        public ActionResult IncluirViagem()
        {
            CadastroViagemVM vm = new CadastroViagemVM();
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<Veiculo>(), "ID", "Placa");
            try
            {
                ViewData["MotivoID"] = new SelectList(new MotivoViagem().List(), "ID", "Descricao");
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
            ViewData["MotivoID"] = new SelectList(Enumerable.Empty<MotivoViagem>(), "ID", "Descricao");
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<Veiculo>(), "ID", "Placa");

            try
            {
                ViewData["MotivoID"] = new SelectList(new MotivoViagem().List(), "ID", "Descricao");
                ViewData["VeiculoID"] = new SelectList(new Veiculo().ListarVeiculosPorIDMotorista(vm.Viagem.MotoristaViagem.ID, true), "ID", "Placa", vm.Viagem.VeiculoViagem.ID);

                vm.VmToModel();
                vm.Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.PROGRAMADA;

                var retorno = ViagemValidador.ValidaInclusao(vm.Viagem);
                if (retorno.Sucesso)
                {
                    vm.Viagem.Create();
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
                vm.Viagem.Read(id);
                vm.Viagem.MotoristaViagem.GetByID(vm.Viagem.MotoristaViagem.ID, true);
                vm.Viagem.VeiculoViagem = new Veiculo().BuscarPorId(vm.Viagem.VeiculoViagem.ID, null);
                vm.ModelToVM();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados da viagem.";
                return RedirectToAction("PainelVigens");
            }
            return View(vm);
        }

        public ActionResult IniciarViagem(int id)
        {
            var Viagem = new Viagem();
            try
            {
                Viagem.Read(id);
                Viagem.Inicio = DateTime.Now;
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.EMANDAMENTO;
                Viagem.Update();

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " iniciada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao iniciar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id = Viagem.ID });
            }
        }
        public ActionResult FinalizarViagem(int id)
        {
            var Viagem = new Viagem();
            try
            {
                Viagem.Read(id);
                Viagem.Fim = DateTime.Now;
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.ENCERRADA;
                Viagem.Update();

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " encerrada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao encerrar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id = Viagem.ID });
            }
        }
        public ActionResult CancelarViagem(int id)
        {
            var Viagem = new Viagem();
            try
            {
                Viagem.Read(id);
                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.CANCELADA;
                Viagem.Update();

                TempData["MensagemSucesso"] = "Viagem: " + Viagem.Codigo + " cancelada! ";
                return RedirectToAction("PainelDeViagens");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao cancelar viagem: " + e.Message;
                return RedirectToAction("AdministrarViagem", new { id = Viagem.ID });
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
            var veiculos = new Veiculo().ListarVeiculosPorIDMotorista(Convert.ToInt32(motoristaID), true);
            return Json(new SelectList(veiculos, "ID", "Placa"));
        }

        public JsonResult SelecionaVeiculo(string veiculoID)
        {
            var veiculo = new Veiculo().BuscarPorId(Convert.ToInt32(veiculoID), true);
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
