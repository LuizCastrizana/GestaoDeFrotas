using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestaoDeFrotas.Enums;
using GestaoDeFrotas.Front.ViewModels;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using X.PagedList;

namespace GestaoDeFrotas.Controllers
{
    public class VeiculoController : Controller
    {
        #region Painel
        public ActionResult PainelDeVeiculos()
        {
            PainelVeiculosVM vm = new PainelVeiculosVM();

            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                };
            var OpcoesFiltragem= new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.PLACA, "Placa"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.DATAINCLUSAO, "Data"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.STATUS, "Status")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");

            vm.Veiculos = Enumerable.Empty<VeiculoVM>().ToPagedList(1, 15);;

            try
            {
                var veiculosDBE = VeiculoTools.BuscarVeiculosPainel(vm.BuscaPlaca, vm.OpcaoOrdenacao, vm.OpcoesFiltragem, vm.Todos);
                vm.CastListaVeiculosParaVM(veiculosDBE);
                vm.Veiculos = vm.Veiculos.ToPagedList(1, 15);

                return View(vm);
            }
            catch(Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel";
                return View(vm);
            }
        }

        public ActionResult BuscarVeiculosPainel(PainelVeiculosVM vm, int? pagina)
        {
            if (vm.BuscaPlaca == null)
                vm.BuscaPlaca = string.Empty;

            var numPagina = pagina ?? 1;

            var OpcoesOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                };
            var OpcoesFiltragem = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.PLACA, "Placa"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.DATAINCLUSAO, "Data"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVEICULOS.STATUS, "Status")
                };
            ViewData["OpcaoOrdenacao"] = new SelectList(OpcoesOrdenacao, "Id", "Text");
            ViewData["OpcoesFiltragem"] = new SelectList(OpcoesFiltragem, "Id", "Text");

            vm.Veiculos = Enumerable.Empty<VeiculoVM>().ToPagedList(numPagina, 15);
            var veiculosVM = new List<VeiculoVM>();

            try
            {
                var veiculosDBE = VeiculoTools.BuscarVeiculosPainel(vm.BuscaPlaca, vm.OpcaoOrdenacao, vm.OpcoesFiltragem, vm.Todos);
                vm.CastListaVeiculosParaVM(veiculosDBE);
                vm.Veiculos = vm.Veiculos.ToPagedList(numPagina, 15);

                return View("PainelDeVeiculos", vm);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel";
             
                return View(vm);
            }
        }
        #endregion

        #region Incluir
        public ActionResult IncluirVeiculo()
        {
            CadastroVeiculoVM vm = new CadastroVeiculoVM();
            try
            {
                ViewData["MarcaID"] = new SelectList(new MarcaVeiculoDAL().ListarMarcas(), "ID", "Nome");
                ViewData["ModeloID"] = new SelectList(new ModeloVeiculoDAL().ListarModelos(), "ID", "Nome");
                ViewData["TipoID"] = new SelectList(new TipoVeiculoDAL().ListarTipos(), "ID", "Descricao");
                return View(vm);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao acessar o banco de dados - " + e.Message;

                return RedirectToAction("PainelDeVeiculos");
            }
        }

        [HttpPost]
        public ActionResult IncluirVeiculo(CadastroVeiculoVM vm)
        {
            vm.VeiculoVM.Marca.ID = vm.MarcaID;
            vm.VeiculoVM.Modelo.ID = vm.ModeloID;
            vm.VeiculoVM.Tipo.ID = vm.TipoID;
            vm.VeiculoVM.Placa = vm.Placa;

            try
            {
                ViewData["MarcaID"] = new SelectList(new MarcaVeiculoDAL().ListarMarcas(), "ID", "Nome", vm.MarcaID);
                ViewData["ModeloID"] = new SelectList(new ModeloVeiculoDAL().ListarModelos(), "ID", "Nome", vm.ModeloID);
                ViewData["TipoID"] = new SelectList(new TipoVeiculoDAL().ListarTipos(), "ID", "Descricao", vm.TipoID);
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["MarcaID"] = new SelectList(Enumerable.Empty<MarcaVeiculoDBE>(), "ID", "Nome");
                ViewData["ModeloID"] = new SelectList(Enumerable.Empty<ModeloVeiculoDBE>(), "ID", "Nome");
                ViewData["TipoID"] = new SelectList(Enumerable.Empty<TipoVeiculoDBE>(), "ID", "Descricao");
            }

            if (ModelState.IsValid)
            {
                try
                {                   
                    //verifica se existe cadastro inativo com a mesma placa
                    if (new VeiculoDAL().BuscarPorPlaca(vm.VeiculoVM.Placa, false).ID != 0)
                    {
                        TempData["MensagemAviso"] = "A Placa inserida está vinculada a este cadastro inativo.";
                        return RedirectToAction("VisualizarVeiculo", new { id = vm.VeiculoVM.ID });
                    }
                    
                    new VeiculoDAL().Create(vm.VeiculoVM.CastToDBE());

                    TempData["MensagemSucesso"] = "Veículo cadastrado com sucesso!";

                    return RedirectToAction("PainelDeVeiculos");
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar o veículo: " + e.Message;

                    return View(vm);
                }
            }
            return View(vm);
        }

        public ActionResult ValidaVeiculoAtivo(string Placa)
        {
            if (new VeiculoDAL().BuscarPorPlaca(Placa.ToUpper(), true).ID > 0)
                return Json("Placa já cadastrada", JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Editar
        public ActionResult VisualizarVeiculo(int id)
        {
            var vm = new CadastroVeiculoVM();

            try
            {
                vm.VeiculoVM.CastFromDBE(new VeiculoDAL().BuscarPorId(id, null));
                vm.VeiculoVM.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(id, false);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do veículo - " + e.Message;

                return RedirectToAction("PainelDeVeiculos");
            }

            return View(vm);
        }

        public ActionResult EditarVeiculo(int id)
        {
            CadastroVeiculoVM vm;

            try
            {
                vm = new CadastroVeiculoVM();
                vm.VeiculoVM.CastFromDBE(new VeiculoDAL().BuscarPorId(id, null));
                vm.DataInclusao = vm.VeiculoVM.DataInclusao.ToString("dd/MM/yyyy HH:mm");
                vm.DataAlteracao = vm.VeiculoVM.DataAlteracao.ToString("dd/MM/yyyy HH:mm");
                vm.VeiculoVM.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(id, true);
                vm.MarcaID = vm.VeiculoVM.Marca.ID;
                vm.ModeloID = vm.VeiculoVM.Modelo.ID;
                vm.TipoID = vm.VeiculoVM.Modelo.ID;

                ViewData["MarcaID"] = new SelectList(new MarcaVeiculoDAL().ListarMarcas(), "ID", "Nome");
                ViewData["ModeloID"] = new SelectList(new ModeloVeiculoDAL().ListarModelos(), "ID", "Nome");
                ViewData["TipoID"] = new SelectList(new TipoVeiculoDAL().ListarTipos(), "ID", "Descricao");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do veículo - " + e.Message;

                return RedirectToAction("PainelDeVeiculos");
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarVeiculo(CadastroVeiculoVM vm)
        {
            vm.VeiculoVM.Marca.ID = vm.MarcaID;
            vm.VeiculoVM.Modelo.ID = vm.ModeloID;
            vm.VeiculoVM.Tipo.ID = vm.TipoID;
            vm.VeiculoVM.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(vm.VeiculoVM.ID, true);

            ViewData["MarcaID"] = new SelectList(Enumerable.Empty<MarcaVeiculoDBE>(), "ID", "Nome");
            ViewData["ModeloID"] = new SelectList(Enumerable.Empty<ModeloVeiculoDBE>(), "ID", "Nome");
            ViewData["TipoID"] = new SelectList(Enumerable.Empty<TipoVeiculoDBE>(), "ID", "Descricao");
            try 
            {
                ViewData["MarcaID"] = new SelectList(new MarcaVeiculoDAL().ListarMarcas(), "ID", "Nome");
                ViewData["ModeloID"] = new SelectList(new ModeloVeiculoDAL().ListarModelos(), "ID", "Nome");
                ViewData["TipoID"] = new SelectList(new TipoVeiculoDAL().ListarTipos(), "ID", "Descricao");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do veículo - " + e.Message;
                return RedirectToAction("PainelDeVeiculos");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    new VeiculoDAL().Update(vm.VeiculoVM.CastToDBE());
                    TempData["MensagemSucesso"] = "Cadastro editado com sucesso!";
                    return RedirectToAction("PainelDeVeiculos");

                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao editar cadastro de veículo - " + e.Message;
                    return View(vm);
                }
            }
            return View(vm);
        }

        public ActionResult DesativarVeiculo(int id)
        {
            try
            {
                new VeiculoDAL().DesativarVeiculo(id);
                TempData["MensagemSucesso"] = "Cadastro desativado com sucesso!";
                return RedirectToAction("EditarVeiculo", new { id });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao desativar cadastro - " + e.Message;
                return RedirectToAction("EditarVeiculo", new { id });
            }
        }

        public ActionResult ReativarVeiculo(int id)
        {
            try
            {
                new VeiculoDAL().ReativarVeiculo(id);
                TempData["MensagemSucesso"] = "Cadastro reativado com sucesso!";
                return RedirectToAction("EditarVeiculo", new { id });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao reativar cadastro - " + e.Message;
                return RedirectToAction("EditarVeiculo", new { id });
            }
        }
        #endregion

        #region Vincular Motorista
        [HttpPost]
        public ActionResult VincularMotorista(CadastroVeiculoVM vm)
        {
            MotoristaDBE motorista = new MotoristaDBE();

            if (vm.BuscaMotorista == null)
                vm.BuscaMotorista = string.Empty;

            try
            {
                vm.VeiculoVM.CastFromDBE(new VeiculoDAL().BuscarPorId(vm.VeiculoVM.ID, true));
                vm.VeiculoVM.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(vm.VeiculoVM.ID, true);
                motorista = new MotoristaDAL().GetByCPF(vm.BuscaMotorista, false);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados de motorista e veículo";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoVM.ID });
            }

            if (vm.VeiculoVM.ListaMotoristas.Any(x => x.CPF == vm.BuscaMotorista))
            {
                TempData["MensagemErro"] = "O motorista informado já está vinculado ao veículo!";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoVM.ID });
            }
            if (motorista.ID == 0)
            {
                TempData["MensagemErro"] = "Motorista não encontrado ou inativo! CPF: " + vm.BuscaMotorista;

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoVM.ID });
            }

            try
            {
                new MotoristaDAL().VincularVeiculoMotorista(vm.VeiculoVM.ID, motorista.ID);

                TempData["MensagemSucesso"] = "Motorista vinculado com sucesso!";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoVM.ID });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao vincular motorista - " + e.Message;

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoVM.ID });
            }
        }

        public ActionResult DesvincularMotoristaVeiculo(int veiculoid, int motoristaid)
        {
            try
            {
                new MotoristaDAL().DesvincularVeiculoMotorista(veiculoid, motoristaid);

                TempData["MensagemSucesso"] = "Motorista desvinculado com sucesso!";

                return RedirectToAction("EditarVeiculo", new { id = veiculoid });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao desvincular motorista - " + e.Message;

                return RedirectToAction("EditarVeiculo", new { id = veiculoid });
            }
        }
        #endregion
    }
}