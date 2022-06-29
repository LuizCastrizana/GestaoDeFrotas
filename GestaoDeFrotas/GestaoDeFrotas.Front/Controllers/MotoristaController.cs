using System;
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
using GestaoDeFrotas.Data.Enums;

namespace GestaoDeFrotas.Controllers
{
    public class MotoristaController : Controller
    {
        #region Painel
        public ActionResult PainelDeMotoristas()
        {
            PainelMotoristasVM vm = new PainelMotoristasVM();

            var OpcoesPainel = MotoristaTools.MontarListasOpcoesPainel();
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesPainel.Ordenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesPainel.Filtros, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcoesPainel.CampoOrdenacao, "Id", "Text");
            vm.Motoristas = Enumerable.Empty<MotoristaVM>().ToPagedList(1, 15);

            try
            {
                var listaMotoristasDBE = new MotoristaDAL().Read(new FiltroBusca()).OrderBy(x => x.PrimeiroNome);
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

            var OpcoesPainel = MotoristaTools.MontarListasOpcoesPainel();
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesPainel.Ordenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesPainel.Filtros, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcoesPainel.CampoOrdenacao, "Id", "Text");

            try
            {
                // remove caracteres da string de busca
                vm.BuscaMotorista = StringTools.RemoverCaracteres(vm.BuscaMotorista, "-.");

                var listaMotoristasDBE = new MotoristaBLL().BuscarDadosPainel(MontarFiltrosDeBusca(vm),
                    (ENUMCAMPOSPAINELMOTORISTAS)vm.OpcaoCampoOrdenacao,
                    (ENUMOPCOESORDENACAO)vm.OpcaoOrdenacao);

                vm.CastListaMotoristasParaVM(listaMotoristasDBE);

                vm.Motoristas = vm.Motoristas.ToPagedList(numPagina, 15);
                return View("PainelDeMotoristas", vm);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel.";
                vm.Motoristas = Enumerable.Empty<MotoristaVM>().ToPagedList(numPagina, 15);
                return View("PainelDeMotoristas", vm);
            }
        }

        private FiltroBusca MontarFiltrosDeBusca(PainelMotoristasVM vm)
        {
            var FiltroBusca = new FiltroBusca();

            switch (vm.OpcoesFiltragem)
            {
                case (int)ENUMCAMPOSPAINELMOTORISTAS.NOME:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("NOMECOMPLETO",  vm.BuscaMotorista, EnumTipoCampo.TEXTO, EnumTipoCondicao.CONTEM)
                            }
                        },
                    };
                    break;
                case (int)ENUMCAMPOSPAINELMOTORISTAS.CPF:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("CPF",  vm.BuscaMotorista, EnumTipoCampo.TEXTO, EnumTipoCondicao.CONTEM)
                            }
                        },
                    };
                    break;
                case (int)ENUMCAMPOSPAINELMOTORISTAS.CNH:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("CNH",  vm.BuscaMotorista, EnumTipoCampo.TEXTO, EnumTipoCondicao.CONTEM)
                            }
                        },
                    };
                    break;
                case (int)ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("MUNICIPIO",  vm.BuscaMotorista, EnumTipoCampo.TEXTO, EnumTipoCondicao.CONTEM)
                            }
                        },
                    };
                    break;
                case (int)ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("DATAINCLUSAO",  StringTools.ConverterEmData(vm.DataInicio, "en-US"), EnumTipoCondicao.MAIOR_OU_IGUAL),
                                new Condicao("DATAINCLUSAO",  StringTools.ConverterEmData(vm.DataFim, "en-US"), EnumTipoCondicao.MENOR_OU_IGUAL)
                            }
                        },
                    };
                    break;
                case (int)ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO:
                    FiltroBusca.ListaExpressao = new List<Expressao>
                    {
                        new Expressao()
                        {
                            OperadorLogico = EnumOperadorLogico.E,
                            ListaCondicao = new List<Condicao>
                            {
                                new Condicao("DATAALTERACAO",  StringTools.ConverterEmData(vm.DataInicio, "en-US"), EnumTipoCondicao.MAIOR_OU_IGUAL),
                                new Condicao("DATAALTERACAO",  StringTools.ConverterEmData(vm.DataFim, "en-US"), EnumTipoCondicao.MENOR_OU_IGUAL)
                            }
                        },
                    };
                    break;
            }

            // exibir inativos
            if (vm.Todos == false)
            {
                var expressao = new Expressao()
                {
                    OperadorLogico = EnumOperadorLogico.E,
                    ListaCondicao = new List<Condicao>
                        {
                            new Condicao("STATUS", true, EnumTipoCondicao.IGUAL)
                        }
                };
                FiltroBusca.ListaExpressao.Add(expressao);
            }

            return FiltroBusca;
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