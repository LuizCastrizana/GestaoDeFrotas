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
using GestaoDeFrotas.Shared.Tools;

namespace GestaoDeFrotas.Controllers
{
    public class MotoristaController : Controller
    {
        #region Painel
        public ActionResult PainelDeMotoristas()
        {
            var BLL = new MotoristaBLL();
            PainelMotoristasVM vm = new PainelMotoristasVM();

            var OpcoesPainel = BLL.MontarMenusPainel();
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesPainel.Ordenacao, "Id", "Text", vm.OpcaoOrdenacao);
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesPainel.Filtros, "Id", "Text", vm.OpcoesFiltragem);
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcoesPainel.CampoOrdenacao, "Id", "Text", vm.OpcaoCampoOrdenacao);
            vm.Motoristas = Enumerable.Empty<MotoristaVM>().ToPagedList(1, 15);

            try
            {
                var listaMotoristasDBE = BLL.BuscarDadosPainel(MontarFiltrosDeBusca(vm), 
                    (ENUMCAMPOSPAINELMOTORISTAS)vm.OpcaoCampoOrdenacao, 
                    (ENUMOPCOESORDENACAO)vm.OpcaoOrdenacao);

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
            var BLL = new MotoristaBLL();

            if (vm.BuscaMotorista == null)
                vm.BuscaMotorista = string.Empty;

            var OpcoesPainel = BLL.MontarMenusPainel();
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesPainel.Ordenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesPainel.Filtros, "Id", "Text");
            ViewData["OpcaoCampoOrdenacao"] = new SelectList(OpcoesPainel.CampoOrdenacao, "Id", "Text");

            try
            {
                vm.BuscaMotorista = StringTools.RemoverCaracteres(vm.BuscaMotorista, "-.");

                var listaMotoristasDBE = BLL.BuscarDadosPainel(MontarFiltrosDeBusca(vm),
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
                var DadosDropdown = new MotoristaBLL().MontarMenusCadastro(false);
                ViewData["MunicipioSelecionado"] = new SelectList(DadosDropdown.Municipios, "ID", "NomeMunicipio");
                ViewData["EstadoSelecionado"] = new SelectList(DadosDropdown.Estados, "ID", "NomeEstado");
                ViewData["CategoriaSelecionada"] = new SelectList(DadosDropdown.CategoriasCNH, "ID", "Categoria");

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
            var Resposta = new RespostaNegocio<MotoristaDBE>();
            vm.CastToMotoristaVM();

            try
            {
                var DadosDropdown = new MotoristaBLL().MontarMenusCadastro(true);
                ViewData["EstadoSelecionado"] = new SelectList(DadosDropdown.Estados, "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["MunicipioSelecionado"] = new SelectList(DadosDropdown.Municipios, "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(DadosDropdown.CategoriasCNH, "ID", "Categoria", vm.CategoriaSelecionada);
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
                vm.CastFromDBE(new MotoristaBLL().BuscarPorID(id));
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
                vm.Motorista.CastFromDBE(new MotoristaBLL().BuscarPorID(id));
                vm.DataNascimento = vm.Motorista.DataNascimento.ToString("yyyy-MM-dd");
                vm.DataEmissaoCNH = vm.Motorista.CNH.DataEmissao.ToString("yyyy-MM-dd");
                vm.DataValidadeCNH = vm.Motorista.CNH.DataValidade.ToString("yyyy-MM-dd");
                var DadosDropDown = new MotoristaBLL().MontarMenusCadastro(true);
                ViewData["MunicipioSelecionado"] = new SelectList(DadosDropDown.Municipios, "ID", "NomeMunicipio", vm.Motorista.Endereco.Municipio.ID.ToString());
                ViewData["EstadoSelecionado"] = new SelectList(DadosDropDown.Estados, "ID", "NomeEstado", vm.Motorista.Endereco.Municipio.Estado.ID.ToString());
                ViewData["CategoriaSelecionada"] = new SelectList(DadosDropDown.CategoriasCNH, "ID", "Categoria", vm.Motorista.CNH.Categoria.ID.ToString());
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
            var BLL = new MotoristaBLL();
            var Resposta = new RespostaNegocio<MotoristaDBE>();
            
            vm.CastToMotoristaVM();
            var motoristaDBE = vm.Motorista.CastToDBE();
            try
            {
                var DadosDropDown = BLL.MontarMenusCadastro(true);
                ViewData["MunicipioSelecionado"] = new SelectList(DadosDropDown.Municipios, "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(DadosDropDown.Estados, "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(DadosDropDown.CategoriasCNH, "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<MunicipioDBE>(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(Enumerable.Empty<EstadoDBE>(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(Enumerable.Empty<CategoriaCNHDBE>(), "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            if (ModelState.IsValid)
            {
                BLL.EditarMotorista(motoristaDBE, ref Resposta);

                if (Resposta.Status != EnumStatusResposta.Sucesso)
                {
                    TempData["MensagemErro"] = Resposta.Mensagem;
                    return View(vm);
                }

                TempData["MensagemSucesso"] = Resposta.Mensagem;
                return RedirectToAction("PainelDeMotoristas");
            }

            return View(vm);
        }

        public ActionResult AlterarStatusMotorista(int ID, bool StatusAtual)
        {
            var Resposta = new RespostaNegocio<MotoristaDBE>();
            new MotoristaBLL().AlterarStatusMotorista(ID, StatusAtual, ref Resposta);

            if (Resposta.Status != EnumStatusResposta.Sucesso)
            {
                TempData["MensagemErro"] = Resposta.Mensagem;
                return RedirectToAction("EditarMotorista", new { ID });
            }

            TempData["MensagemSucesso"] = Resposta.Mensagem;
            return RedirectToAction("EditarMotorista", new { ID });
        }
        #endregion

        #region Vincular Veiculo
        [HttpPost]
        public ActionResult VincularVeiculo(CadastroMotoristaVM vm)
        {
            var Resposta = new RespostaNegocio<MotoristaDBE>();

            if (vm.BuscaPlaca == null)
                vm.BuscaPlaca = string.Empty;

            new MotoristaBLL().VincularVeiculo(vm.Motorista.ID, vm.BuscaPlaca, ref Resposta);

            if (Resposta.Status != EnumStatusResposta.Sucesso)
            {
                TempData["MensagemErro"] = Resposta.Mensagem;
                return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
            }

            TempData["MensagemSucesso"] = Resposta.Mensagem;
            return RedirectToAction("EditarMotorista", new { id = vm.Motorista.ID });
        }

        public ActionResult DesvincularVeiculoMotorista(int veiculoid, int motoristaid)
        {
            var Resposta = new RespostaNegocio<MotoristaDBE>();
            new MotoristaBLL().DesvincularVeiculoMotorista(veiculoid, motoristaid, ref Resposta);

            if (Resposta.Status != EnumStatusResposta.Sucesso)
            {
                TempData["MensagemErro"] = Resposta.Mensagem;
                return RedirectToAction("EditarMotorista", new { id = motoristaid });
            }
            TempData["MensagemSucesso"] = Resposta.Mensagem;
            return RedirectToAction("EditarMotorista", new { id = motoristaid });
        }
        #endregion

        #region Busca JSON
        [WebMethod]
        public JsonResult BuscaMunicipio(int idEstado)
        {
            var ListaMunicipios = new MotoristaBLL().BuscarMunicipiosPorEstado(idEstado);
            return Json(new SelectList(ListaMunicipios, "ID", "NomeMunicipio"));
        }
        public ActionResult ValidaCpf(MotoristaDBE motorista)
        {
            MotoristaDBE motoristaBusca;
            var ResultadoBusca = new MotoristaBLL().BuscarPorNomeOuCPF(motorista.CPF, false);
            if (ResultadoBusca.Any())
            {
                motoristaBusca = ResultadoBusca.First();
                if (motoristaBusca.ID > 0 && motoristaBusca.ID != motorista.ID)
                    return Json("CPF já cadastrado", JsonRequestBehavior.AllowGet);
            }
            if (!CPFTools.ValidaCpf(motorista.CPF))
            {
                return Json("CPF inválido", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }       
}