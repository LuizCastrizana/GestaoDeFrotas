using CadastroDeCaminhoneiro.Enums;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadastroDeCaminhoneiro
{ 
    public static class ViagemTools
    {
        public static IEnumerable<ViagemDBE> BuscarViagensPainel(ViagemDBE ViagemBusca, string busca, int opcaoFiltragem, int opcaoCampoOrdenacao, int opcaoOrdenacao)
        {
            var lista = Enumerable.Empty<ViagemDBE>();
            if (busca == null)
                busca = string.Empty;

            switch (opcaoFiltragem)
            {
                case (int)ENUMCAMPOSPAIELVIAGEM.CODIGO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.Codigo.Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.DataInclusao.ToString("dd/MM/yyyy").Contains(busca));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.STATUS:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.ViagemStatus.ID.ToString().Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.VEICULO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => StringTools.RemoverCaracteres(m.VeiculoViagem.Placa.ToUpper(), "-").Contains(StringTools.RemoverCaracteres(busca.ToUpper(), "-")));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA:
                    lista = new ViagemDAL().Read(ViagemBusca).
                        Where(m => (m.MotoristaViagem.PrimeiroNome + " " + m.MotoristaViagem.Sobrenome).ToUpper().Contains(busca.ToUpper())
                        ||
                        (StringTools.RemoverCaracteres(m.MotoristaViagem.CPF, "-.")).Contains(StringTools.RemoverCaracteres(busca, "-.")));
                    break;
                default:
                    lista = new ViagemDAL().List(false);
                    break;
            }

            if (opcaoOrdenacao == (int)ENUMOPCOESORDENACAO.CRESCENTE)
            {
                switch (opcaoCampoOrdenacao)
                {
                    case (int)ENUMCAMPOSPAIELVIAGEM.CODIGO:
                        lista = lista.OrderBy(m => m.Codigo);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO:
                        lista = lista.OrderBy(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.STATUS:
                        lista = lista.OrderBy(m => m.ViagemStatus.ID);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.VEICULO:
                        lista = lista.OrderBy(m => m.VeiculoViagem.Placa);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA:
                        lista = lista.OrderBy(m => m.MotoristaViagem.PrimeiroNome);
                        break;
                    default:
                        lista = lista.OrderBy(m => m.ID);
                        break;
                }
            }
            else
            {
                switch (opcaoCampoOrdenacao)
                {
                    case (int)ENUMCAMPOSPAIELVIAGEM.CODIGO:
                        lista = lista.OrderByDescending(m => m.Codigo);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO:
                        lista = lista.OrderByDescending(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.STATUS:
                        lista = lista.OrderByDescending(m => m.ViagemStatus.ID);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.VEICULO:
                        lista = lista.OrderByDescending(m => m.VeiculoViagem.Placa);
                        break;
                    case (int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA:
                        lista = lista.OrderByDescending(m => m.MotoristaViagem.PrimeiroNome);
                        break;
                    default:
                        lista = lista.OrderByDescending(m => m.ID);
                        break;
                }
            }
            return lista;
        }

        public static FiltrosPainelViagem MontarListasOpcoesPainel()
        {
            var retorno = new FiltrosPainelViagem
            {
                Ordenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                },
                Filtros = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                },
                CampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.STATUS, "Status"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA, "Motorista")
                },
                StatusViagem = new List<DropDownItem>
                {
                    new DropDownItem(0, "Todos"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.PROGRAMADA, "Programada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.EMANDAMENTO, "Em Andamento"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.ENCERRADA, "Encerrada"),
                    new DropDownItem((int)ENUMSTATUSVIAGEM.CANCELADA, "Cancelada")
                }
            };

            return retorno;
        }

        public static void GerarCodigoViagem(ViagemDBE viagem)
        {
            const int CONTADORINICIAL = 1;
            const int CONTADORMAXIMO = 99999;
            if (viagem.Motivo.ID == 0)
                throw new Exception("Não foi possível gerar um novo código de viagem: Campo motivo não informado.");

            // Inicia o código com as duas primeiras letras do motivo e a data atual
            viagem.Codigo =
                viagem.Motivo.Descricao.Substring(0, 2).ToUpper() +
                DateTime.Now.ToString("yy") +
                DateTime.Now.ToString("dd") +
                DateTime.Now.ToString("MM");

            // Busca viagens geradas na data atual e com o mesmo motivo, pois nesse caso o início do código será igual ao cógigo que está sendo gerado
            var viagemBusca = new ViagemDBE()
            {
                Motivo = viagem.Motivo
            };
            var viagensDia = new ViagemDAL().Read(viagemBusca).Where(m => m.DataInclusao.Date == DateTime.Now.Date);

            // Se houver qualquer viagem gerada na data atual e com o mesmo motivo, verifica o valor do contador do código da viagem mais recente e incrementa o valor
            // Em seguida concatena o valor do contador ao código da viagem
            if (viagensDia.Any())
            {
                var ultimaViagem = viagensDia.OrderByDescending(v => v.ID).First();
                var contador = Convert.ToInt32(ultimaViagem.Codigo.Substring(8, 5));
                if (contador == CONTADORMAXIMO)
                    throw new Exception("Não foi possível gerar um novo código de viagem: limite excedido.");
                contador++;
                viagem.Codigo += contador.ToString("D5");
            }
            // Se não, concatena o contador inicial
            else
                viagem.Codigo += CONTADORINICIAL.ToString("D5");
        }
    }
}