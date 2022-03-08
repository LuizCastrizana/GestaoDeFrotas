using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadastroDeCaminhoneiro.Enums;
using CadastroDeCaminhoneiro.Models;
using CadastroDeCaminhoneiro.ViewModels;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using X.PagedList;

namespace CadastroDeCaminhoneiro.Controllers
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

            vm.Veiculos = Enumerable.Empty<VeiculoDBE>().ToPagedList(1, 10);

            try
            {
                vm.Veiculos = VeiculoHelper.BuscarVeiculosPainel(vm.BuscaPlaca, vm.OpcaoOrdenacao, vm.OpcoesFiltragem, vm.Todos).ToPagedList(1, 10);

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

            vm.Veiculos = Enumerable.Empty<VeiculoDBE>().ToPagedList(numPagina, 10);

            try
            {
                vm.Veiculos = VeiculoHelper.BuscarVeiculosPainel(vm.BuscaPlaca, vm.OpcaoOrdenacao, vm.OpcoesFiltragem, vm.Todos).ToPagedList(numPagina, 10);

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
            vm.VeiculoDBE.Marca.ID = vm.MarcaID;
            vm.VeiculoDBE.Modelo.ID = vm.ModeloID;
            vm.VeiculoDBE.Tipo.ID = vm.TipoID;
            vm.VeiculoDBE.Placa = vm.Placa;

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
                    if (new VeiculoDAL().BuscarPorPlaca(vm.VeiculoDBE.Placa, false).ID != 0)
                    {
                        TempData["MensagemAviso"] = "A Placa inserida está vinculada a este cadastro inativo.";
                        return RedirectToAction("VisualizarVeiculo", new { id = vm.VeiculoDBE.ID });
                    }

                    new VeiculoDAL().Create(vm.VeiculoDBE);

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
                vm.VeiculoDBE = new VeiculoDAL().BuscarPorId(id, null);
                vm.VeiculoDBE.Marca = new MarcaVeiculoDAL().BuscarPorId(vm.VeiculoDBE.Marca.ID);
                vm.VeiculoDBE.Modelo = new ModeloVeiculoDAL().BuscarPorId(vm.VeiculoDBE.Modelo.ID);
                vm.VeiculoDBE.Tipo = new TipoVeiculoDAL().BuscarPorId(vm.VeiculoDBE.Tipo.ID);
                vm.VeiculoDBE.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(id, false);
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
                vm.VeiculoDBE = new VeiculoDAL().BuscarPorId(id, null);
                vm.DataInclusao = vm.VeiculoDBE.DataInclusao.ToString("dd/MM/yyyy HH:mm");
                vm.DataAlteracao = vm.VeiculoDBE.DataAlteracao.ToString("dd/MM/yyyy HH:mm");
                vm.VeiculoDBE.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(id, true);
                vm.MarcaID = vm.VeiculoDBE.Marca.ID;
                vm.ModeloID = vm.VeiculoDBE.Modelo.ID;
                vm.TipoID = vm.VeiculoDBE.Modelo.ID;

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
            vm.VeiculoDBE.Marca.ID = vm.MarcaID;
            vm.VeiculoDBE.Modelo.ID = vm.ModeloID;
            vm.VeiculoDBE.Tipo.ID = vm.TipoID;
            vm.VeiculoDBE.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(vm.VeiculoDBE.ID, true);

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
                    new VeiculoDAL().Update(vm.VeiculoDBE);
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
                vm.VeiculoDBE = new VeiculoDAL().BuscarPorId(vm.VeiculoDBE.ID, true);
                vm.VeiculoDBE.ListaMotoristas = new MotoristaDAL().ListByVeiculoID(vm.VeiculoDBE.ID, true);
                motorista = new MotoristaDAL().GetByCPF(vm.BuscaMotorista, false);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados de motorista e veículo";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoDBE.ID });
            }

            if (vm.VeiculoDBE.ListaMotoristas.Any(x => x.CPF == vm.BuscaMotorista))
            {
                TempData["MensagemErro"] = "O motorista informado já está vinculado ao veículo!";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoDBE.ID });
            }
            if (motorista.ID == 0)
            {
                TempData["MensagemErro"] = "Motorista não encontrado ou inativo! CPF: " + vm.BuscaMotorista;

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoDBE.ID });
            }

            try
            {
                new MotoristaDAL().VincularVeiculoMotorista(vm.VeiculoDBE.ID, motorista.ID);

                TempData["MensagemSucesso"] = "Motorista vinculado com sucesso!";

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoDBE.ID });
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao vincular motorista - " + e.Message;

                return RedirectToAction("EditarVeiculo", new { id = vm.VeiculoDBE.ID });
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