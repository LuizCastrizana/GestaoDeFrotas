using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestaoDeFrotas.Front.ViewModels;
using GestaoDeFrotas.Models;

namespace GestaoDeFrotas.Controllers
{
    public class ConfiguracaoController : Controller
    {
        // GET: Configuracao
        public ActionResult PopularBD()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PopularBDEstados(string caminho)
        {
            caminho += "/estados.csv";
            LeitorCSV leitor = new LeitorCSV(caminho);
            IEnumerable<Estado> listaEstados;
            int contador = 0;
            try
            {
                listaEstados = leitor.LerEstados();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao ler arquivo: " + e.Message;
                return RedirectToAction("PopularBD", "Configuracao");
            }
            foreach (var estado in listaEstados)
            {
                try
                {
                    estado.Create();
                    contador++;
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = contador + " inseridos. Erro ao inserir estado: " + estado.NomeEstado + ": " + e.Message;
                    return RedirectToAction("PopularBD", "Configuracao");
                }
            }
            TempData["MensagemSucesso"] = "Estados inseridos: " + contador;
            return RedirectToAction("PopularBD", "Configuracao");
        }
        [HttpPost]
        public ActionResult PopularBDMunicipios(string caminho)
        {
            caminho += "/municipios.csv";
            LeitorCSV leitor = new LeitorCSV(caminho);
            IEnumerable<Municipio> listaMunicipios;
            int contador = 0;
            try
            {
                listaMunicipios = leitor.LerMunicipios();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao ler arquivo: " + e.Message;
                return RedirectToAction("PopularBD", "Configuracao");
            }
            foreach (var municipio in listaMunicipios)
            {
                try
                {
                    municipio.Create();
                    contador++;
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = contador + " inseridos. Erro ao inserir municipio: " + municipio.NomeMunicipio + ": " + e.Message;
                    return RedirectToAction("PopularBD", "Configuracao");
                }
            }
            TempData["MensagemSucesso"] = "Municipios inseridos: " + contador;
            return RedirectToAction("PopularBD", "Configuracao");
        }
        public ActionResult DetalhesVeiculo()
        {
            DetalhesVeiculoVM vmDetalhesVeiculo = new DetalhesVeiculoVM();
            IEnumerable<DetalheVeiculo> ListaDetalhes = Enumerable.Empty<DetalheVeiculo>();
            var tipoDetalhe = new List<DropDownItem>
                {
                    new DropDownItem(1, "Marca"),
                    new DropDownItem(2, "Modelo"),
                    new DropDownItem(3, "Tipo")
                };
            ViewData["TipoDetalhe"] = new SelectList(tipoDetalhe, "Id", "Text");
            ViewData["DetalheSelecionado"] = new SelectList(ListaDetalhes, "ID", "NomeEstado");
            
            return View(vmDetalhesVeiculo);
        }
        [HttpPost]
        public ActionResult IncluirDetalhe(DetalhesVeiculoVM vm)
        {
            vm.Detalhe.TipoDetalhe = vm.TipoDetalhe;
            try
            {
                vm.Detalhe.InserirDetalhe();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao incluir item! " + e.Message;
                return RedirectToAction("DetalhesVeiculo");
            }
            TempData["MensagemSucesso"] = "Item \'" + vm.Detalhe.Descricao + "\' inserido com suceso!";
            return RedirectToAction("DetalhesVeiculo");
        }
        [HttpPost]
        public ActionResult ExcluirDetalhe(DetalhesVeiculoVM vm)
        {
            DetalheVeiculo detalhe = new DetalheVeiculo()
            {
                ID = vm.DetalheSelecionado,
                TipoDetalhe = vm.TipoDetalhe
            };
            try
            {
                detalhe.ExcluirDetalhe();
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao excluir item! " + e.Message;
                return RedirectToAction("DetalhesVeiculo");
            }
            TempData["MensagemSucesso"] = "Item excluido com suceso!";
            return RedirectToAction("DetalhesVeiculo");
        }
        public JsonResult BuscaDetalhe(int TipoDetalhe)
        {
            IEnumerable<DetalheVeiculo> ListaDetalhes = new DetalheVeiculo().ListarDetalhe(TipoDetalhe)
                .OrderBy(x => x.Descricao);
            return Json(new SelectList(ListaDetalhes, "ID", "Descricao"));
        }
    }
}