using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Front.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Business.BLL;
using GestaoDeFrotas.Business;

namespace GestaoDeFrotas.Controllers
{
    public class ViagemController : Controller
    {
        private readonly ViagemBLL _BLL = new ViagemBLL();
        private readonly VeiculoBLL _VeiculoBLL = new VeiculoBLL();

        #region Painel
        public ActionResult PainelDeViagens()
        {
            PainelViagensVM vm = new PainelViagensVM();

            var opcoesPainel = _BLL.MontarListasOpcoesPainel();

            ViewData["OpcaoOrdenacao"] = new SelectList(opcoesPainel.Ordenacao, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(opcoesPainel.CampoOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(opcoesPainel.Filtros, "Id", "Text");
            ViewData["StatusViagem"] = new SelectList(opcoesPainel.StatusViagem, "Id", "Text");

            vm.OpcoesFiltragem = (int)ENUMCAMPOSPAINELVIAGEM.CODIGO;
            vm.OpcaoCampoOrdenacao = (int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO;
            vm.OpcaoOrdenacao = (int)ENUMOPCOESORDENACAO.DECRESCENTE;

            vm.Viagens = Enumerable.Empty<ViagemDBE>().ToPagedList(1, 15);

            try
            {
                vm.Viagens = _BLL.BuscarViagensPainel(new ViagemDBE(), vm.BuscaViagem, vm.OpcoesFiltragem, vm.OpcaoCampoOrdenacao, vm.OpcaoOrdenacao).ToPagedList(1, 15);
                vm.Viagens.ToPagedList(1, 15);
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

            var opcoesPainel = _BLL.MontarListasOpcoesPainel();

            ViewData["OpcaoOrdenacao"] = new SelectList(opcoesPainel.Ordenacao, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(opcoesPainel.CampoOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(opcoesPainel.Filtros, "Id", "Text");
            ViewData["StatusViagem"] = new SelectList(opcoesPainel.StatusViagem, "Id", "Text");

            vm.Viagens = Enumerable.Empty<ViagemDBE>().ToPagedList(numPagina, 15);

            try
            {
                var ViagemBusca = new ViagemDBE();

                ViagemBusca.ViagemStatus.ID = vm.StatusViagem;

                vm.Viagens = _BLL.BuscarViagensPainel(ViagemBusca, vm.BuscaViagem, vm.OpcoesFiltragem, vm.OpcaoCampoOrdenacao, vm.OpcaoOrdenacao);

                vm.Viagens = vm.Viagens.ToPagedList(numPagina, 15);

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
            var Resposta = new RespostaNegocio<ViagemDBE>();
            try
            {
                _BLL.BuscarViagemPorID(id, ref Resposta);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados da viagem: " + e.Message;
                return RedirectToAction("PainelDeViagens");
            }
            return View(Resposta.Retorno);
        }

        public ActionResult IncluirViagem()
        {
            var Resposta = new RespostaNegocio<IEnumerable<MotivoViagemDBE>>();
            CadastroViagemVM vm = new CadastroViagemVM();
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<VeiculoDBE>(), "ID", "Placa");
            try
            {
                _BLL.ListarMotivos(ref Resposta);
                ViewData["MotivoID"] = new SelectList(Resposta.Retorno, "ID", "Descricao");
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
            var RespostaInclusao = new RespostaNegocio<ViagemDBE>();
            var RespostaMotivos = new RespostaNegocio<IEnumerable<MotivoViagemDBE>>();
            var RespostaVeiculos = new RespostaNegocio<IEnumerable<VeiculoDBE>>();
            ViewData["MotivoID"] = new SelectList(Enumerable.Empty<MotivoViagemDBE>(), "ID", "Descricao");
            ViewData["VeiculoID"] = new SelectList(Enumerable.Empty<VeiculoDBE>(), "ID", "Placa");

            _BLL.ListarMotivos(ref RespostaMotivos);
            _VeiculoBLL.ListarVeiculosPorIDMotorista(vm.MotoristaID, true, ref RespostaVeiculos);
            ViewData["MotivoID"] = new SelectList(RespostaMotivos.Retorno, "ID", "Descricao");
            ViewData["VeiculoID"] = new SelectList(RespostaVeiculos.Retorno, "ID", "Placa", vm.VeiculoID);
            
            vm.VmToDBE();

            _BLL.IncluirViagem(vm.Viagem, ref RespostaInclusao);

            if (RespostaInclusao.Status == EnumStatusResposta.ErroValidacao || RespostaInclusao.Status == EnumStatusResposta.Aviso)
            {
                TempData["MensagemAviso"] = RespostaInclusao.Mensagem;
                return View(vm);
            }
            if (RespostaInclusao.Status == EnumStatusResposta.Erro)
            {
                TempData["MensagemErro"] = RespostaInclusao.Mensagem;
                return View(vm);
            }
            TempData["MensagemSucesso"] = RespostaInclusao.Mensagem;
            return RedirectToAction("PainelDeViagens");
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
            var lista = new MotoristaBLL().BuscarPorNomeOuCPF(busca, false).OrderByDescending(m => m.ID);

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
            foreach (var item in new MotoristaBLL().BuscarPorNomeOuCPF(busca, false).OrderByDescending(m => m.ID))
            {
                lista.Add(new AutoCompleteMotorista(item));
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
