using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using CadastroDeCaminhoneiro.Models;
using CadastroDeCaminhoneiro.ViewModels;
using X.PagedList;

namespace CadastroDeCaminhoneiro.Controllers
{
    public class MotoristaController : Controller
    {
        // GET: Motorista
        #region Painel
        public ActionResult PainelDeMotoristas()
        {
            PainelMotoristasVM viewModelPainel = new PainelMotoristasVM();
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
            viewModelPainel.Motoristas = Enumerable.Empty<Motorista>().ToPagedList(1, 10);
            try
            {
                viewModelPainel.Motoristas = new Motorista().List(false).OrderBy(x => x.PrimeiroNome);
                viewModelPainel.Motoristas = viewModelPainel.Motoristas.ToPagedList(1, 10);
                viewModelPainel.BuscaMotorista = "";
                viewModelPainel.Todos = false;
                return View(viewModelPainel);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel." + e.Message;
                return View(viewModelPainel);
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
                vm.Motoristas = new Motorista().List(vm.Todos)
                                                      .Where(m => (m.PrimeiroNome + " " + m.Sobrenome).ToUpper()
                                                      .Contains(vm.BuscaMotorista.ToUpper())
                                                      ||
                                                     (StringTools.RemoverCaracteres(m.CPF, "-."))
                                                     .Contains(vm.BuscaMotorista));
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
                vm.Motoristas = vm.Motoristas.ToPagedList(numPagina, 10);
                return View("PainelDeMotoristas", vm);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do painel.";
                vm.Motoristas = Enumerable.Empty<Motorista>().ToPagedList(numPagina, 10);
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
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<Municipio>(), "ID", "NomeMunicipio");
                ViewData["EstadoSelecionado"] = new SelectList(new Estado().List(), "ID", "NomeEstado");
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNH().List(), "ID", "Categoria");

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
            vm.VmToModel();

            try
            {
                ViewData["EstadoSelecionado"] = new SelectList(new Estado().List(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["MunicipioSelecionado"] = new SelectList(new Municipio().ListarMunicipios(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNH().List(), "ID", "Categoria", vm.CategoriaSelecionada);
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["EstadoSelecionado"] = new SelectList(Enumerable.Empty<Municipio>(), "ID", "NomeEstado");
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<Estado>(), "ID", "NomeMunicipio");
                ViewData["CategoriaSelecionada"] = new SelectList(Enumerable.Empty<CategoriaCNH>(), "ID", "Categoria");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int idMotorista = 0;
                    // Verifica se existe cadastro inativo com o mesmo CPF
                    vm.Motorista.GetByCPF(vm.Motorista.CPF, null);
                    if (vm.Motorista.ID != 0)
                    {
                        TempData["MensagemAviso"] = "O CPF inserido está vinculado a este cadastro inativo!";
                        return RedirectToAction("VisualizarMotorista", new { id = vm.Motorista.ID });
                    }
                    // Insere endereço
                    vm.Motorista.Endereco.Create();
                    // Verifica se existe CNH
                    switch (vm.Motorista.CNH.VerificaSeEstaCadastrado(ref idMotorista))
                    {
                        // Não existe CNH com os mesmos dados
                        case false:
                            vm.Motorista.CNH.Create();
                            vm.Motorista.Create();
                            TempData["MensagemSucesso"] = "Motorista cadastrado com sucesso!";
                            return RedirectToAction("PainelDeMotoristas");
                        // Existe CNH associada a um motorista
                        case true:
                            TempData["MensagemAviso"] = "Um ou mais dados da CNH inseridos estão associados a este motorista!";
                            return RedirectToAction("VisualizarMotorista", new { id = idMotorista });
                        // Existe CNH cadastrada mas não está vinculada a nenhum motorista (houve uma tentativa de cadastro do motorista mas ocorreu um erro)
                        case null:
                            vm.Motorista.Create();
                            TempData["MensagemSucesso"] = "Motorista cadastrado com sucesso!";
                            return RedirectToAction("PainelDeMotoristas");
                    }
                }
                // Verificar inner exception
                catch (CadastroEnderecoException e)
                {
                    TempData["MensagemErro"] = e.Message;
                    return View(vm);
                }
                catch (CadastroCNHException e)
                {
                    TempData["MensagemErro"] = e.Message;

                    if (!this.ExcluirEndereco(vm.Motorista.Endereco.ID))
                    {
                        TempData["MensagemErro"] = e.Message + " / Erro ao excluir endereço com ID: " + vm.Motorista.Endereco.ID;
                        return View(vm);
                    }

                    return View(vm);
                }
                catch (CadastroMotoristaException e)
                {
                    TempData["MensagemErro"] = e.Message + " " + e.InnerException.Message;

                    if (!this.ExcluirEndereco(vm.Motorista.Endereco.ID))
                    {
                        TempData["MensagemErro"] = e.Message + " / Erro ao excluir endereço com ID: " + vm.Motorista.Endereco.ID;
                        return View(vm);
                    }

                    return View(vm);
                }
            }

            return View(vm);
        }
        #endregion

        #region Editar
        public ActionResult VisualizarMotorista(int id)
        {
            Motorista motorista = new Motorista();
            try
            {
                motorista.GetByID(id, true);
                motorista.Endereco.Municipio = new Municipio().ListarMunicipios().Where(m => m.ID == motorista.Endereco.Municipio.ID).First();
                motorista.ListaVeiculos = new Veiculo().ListarVeiculosPorIDMotorista(id, true);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao buscar dados do motorista - " + e.Message;
                return RedirectToAction("PainelDeMotoristas");
            }
            return View(motorista);
        }

        public ActionResult EditarMotorista(int id)
        {
            CadastroMotoristaVM vm = new CadastroMotoristaVM();
            try
            {
                vm.Motorista.GetByID(id, true);
                vm.Motorista.ListaVeiculos = new Veiculo().ListarVeiculosPorIDMotorista(id, true);
                vm.DataNascimento = vm.Motorista.DataNascimento.ToString("yyyy-MM-dd");
                vm.DataEmissao = vm.Motorista.CNH.DataEmissao.ToString("yyyy-MM-dd");
                vm.DataValidade = vm.Motorista.CNH.DataValidade.ToString("yyyy-MM-dd");
                ViewData["MunicipioSelecionado"] = new SelectList(new Municipio().ListarMunicipios(), "ID", "NomeMunicipio", vm.Motorista.Endereco.Municipio.ID.ToString());
                ViewData["EstadoSelecionado"] = new SelectList(new Estado().List(), "ID", "NomeEstado", vm.Motorista.Endereco.Municipio.Estado.ID.ToString());
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNH().List(), "ID", "Categoria", vm.Motorista.CNH.Categoria.ID.ToString());
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
            vm.VmToModel();
            try
            {
                vm.Motorista.ListaVeiculos = new Veiculo().ListarVeiculosPorIDMotorista(vm.Motorista.ID, true);
                ViewData["MunicipioSelecionado"] = new SelectList(new Municipio().ListarMunicipios(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(new Estado().List(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(new CategoriaCNH().List(), "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException)
            {
                ViewData["MunicipioSelecionado"] = new SelectList(Enumerable.Empty<Municipio>(), "ID", "NomeMunicipio", vm.MunicipioSelecionado);
                ViewData["EstadoSelecionado"] = new SelectList(Enumerable.Empty<Estado>(), "ID", "NomeEstado", vm.EstadoSelecionado);
                ViewData["CategoriaSelecionada"] = new SelectList(Enumerable.Empty<CategoriaCNH>(), "ID", "Categoria", vm.CategoriaSelecionada.ToString());
            }
            if (ModelState.IsValid)
            {
                try
                {
                    vm.Motorista.Endereco.Update();
                    vm.Motorista.CNH.Update();
                    vm.Motorista.Update();
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
                new Motorista().UpdateStatus(id, !status);
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
            Veiculo veiculo;
            if (vm.BuscaPlaca == null)
                vm.BuscaPlaca = string.Empty;
            try
            {
                vm.Motorista.GetByID(vm.Motorista.ID, true);
                vm.Motorista.ListaVeiculos = new Veiculo().ListarVeiculosPorIDMotorista(vm.Motorista.ID, true);
                veiculo = new Veiculo().BuscarPorPlaca(vm.BuscaPlaca.ToUpper(), true);

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException e)
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
                vm.Motorista.VincularVeiculo(veiculo.ID);
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
                new Motorista().DesvincularVeiculoMotorista(veiculoid, motoristaid);
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
            IEnumerable<Municipio> ListaMunicipios = new Municipio().ListarMunicipios();
            return Json(new SelectList(ListaMunicipios
                .Where(x => x.Estado.ID == idEstado)
                .OrderBy(x => x.NomeMunicipio)
                , "ID", "NomeMunicipio"));
        }
        public ActionResult ValidaCpf(Motorista motorista)
        {
            int idEntrada = motorista.ID;
            motorista.GetByCPF(motorista.CPF, false);
            if (motorista.ID > 0 && motorista.ID != idEntrada)
                return Json("CPF já cadastrado", JsonRequestBehavior.AllowGet);
            if (!CPF.ValidaCpf(motorista.CPF))
            {
                return Json("CPF inválido", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private bool ExcluirEndereco(int id)
        {
            try
            {
                new Endereco().Delete(id);
                return true;
            }
            catch (CadastroEnderecoException)
            {
                return false;
            }
        }
    }       
}