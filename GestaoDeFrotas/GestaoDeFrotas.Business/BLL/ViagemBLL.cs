﻿using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Shared.FiltroBusca;
using GestaoDeFrotas.Shared.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business.BLL
{
    public class ViagemBLL
    {
        private readonly ViagemDAL _viagemDAL;
        private readonly MotivoViagemDAL _motivoViagemDAL;
        private readonly StatusViagemDAL _statusViagemDAL;

        public ViagemBLL()
        {
            _viagemDAL = new ViagemDAL();
            _statusViagemDAL = new StatusViagemDAL();
            _motivoViagemDAL = new MotivoViagemDAL();
        }

        #region Cadastro

        public void ListarMotivos(ref RespostaNegocio<IEnumerable<MotivoViagemDBE>> Resposta)
        {
            Resposta.Retorno = _motivoViagemDAL.List();
        }

        public void IncluirViagem(ViagemDBE Viagem, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            try
            {
                GerarCodigoViagem(Viagem);

                Viagem.ViagemStatus.ID = (int)ENUMSTATUSVIAGEM.PROGRAMADA;

                ValidarInclusaoViagem(Viagem, ref Resposta);
                if (Resposta.Status == EnumStatusResposta.Sucesso)
                {
                    new ViagemDAL().Create(Viagem);
                    Resposta.Mensagem = "Viagem cadastrada com sucesso!";
                }
            }
            catch (Exception e)
            {
                Resposta.Status = EnumStatusResposta.Erro;
                Resposta.Mensagem = "Erro ao cadastrar viagem: " + e.Message;
            }
        }

        private void GerarCodigoViagem(ViagemDBE viagem)
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
            var viagensDia = _viagemDAL.Read(viagemBusca).Where(m => m.DataInclusao.Date == DateTime.Now.Date);

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

        #endregion

        #region Busca

        public IEnumerable<ViagemDBE> BuscarViagensPainel(ViagemDBE ViagemBusca, string busca, int opcaoFiltragem, int opcaoCampoOrdenacao, int opcaoOrdenacao)
        {
            var lista = Enumerable.Empty<ViagemDBE>();
            if (busca == null)
                busca = string.Empty;

            switch (opcaoFiltragem)
            {
                case (int)ENUMCAMPOSPAINELVIAGEM.CODIGO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.Codigo.Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.DataInclusao.ToString("dd/MM/yyyy").Contains(busca));
                    break;
                case (int)ENUMCAMPOSPAINELVIAGEM.STATUS:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => m.ViagemStatus.ID.ToString().Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAINELVIAGEM.VEICULO:
                    lista = new ViagemDAL().Read(ViagemBusca).Where(m => StringTools.RemoverCaracteres(m.VeiculoViagem.Placa.ToUpper(), "-").Contains(StringTools.RemoverCaracteres(busca.ToUpper(), "-")));
                    break;
                case (int)ENUMCAMPOSPAINELVIAGEM.MOTORISTA:
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
                    case (int)ENUMCAMPOSPAINELVIAGEM.CODIGO:
                        lista = lista.OrderBy(m => m.Codigo);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO:
                        lista = lista.OrderBy(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.STATUS:
                        lista = lista.OrderBy(m => m.ViagemStatus.ID);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.VEICULO:
                        lista = lista.OrderBy(m => m.VeiculoViagem.Placa);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.MOTORISTA:
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
                    case (int)ENUMCAMPOSPAINELVIAGEM.CODIGO:
                        lista = lista.OrderByDescending(m => m.Codigo);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO:
                        lista = lista.OrderByDescending(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.STATUS:
                        lista = lista.OrderByDescending(m => m.ViagemStatus.ID);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.VEICULO:
                        lista = lista.OrderByDescending(m => m.VeiculoViagem.Placa);
                        break;
                    case (int)ENUMCAMPOSPAINELVIAGEM.MOTORISTA:
                        lista = lista.OrderByDescending(m => m.MotoristaViagem.PrimeiroNome);
                        break;
                    default:
                        lista = lista.OrderByDescending(m => m.ID);
                        break;
                }
            }
            return lista;
        }

        public void BuscarViagemPorID(int id, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            try
            {
                Resposta.Retorno = _viagemDAL.Read(id);
            }
            catch (Exception e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;
            }
        }
        public DropDownMenuPainelViagens MontarListasOpcoesPainel()
        {
            var retorno = new DropDownMenuPainelViagens
            {
                Ordenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                },
                Filtros = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.MOTORISTA, "Motorista")
                },
                CampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.CODIGO, "Codigo"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.STATUS, "Status"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.VEICULO, "Veiculo"),
                    new DropDownItem((int)ENUMCAMPOSPAINELVIAGEM.MOTORISTA, "Motorista")
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

        #endregion

        #region Validações

        public static void ValidarInclusaoViagem(ViagemDBE viagem, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            Resposta.Status = EnumStatusResposta.Sucesso;
            Resposta.Mensagem = "";

            ValidaVinculo(viagem.MotoristaViagem.ID, viagem.VeiculoViagem.ID, ref Resposta);
            if (Resposta.Status != EnumStatusResposta.Sucesso)
                return;

            ValidaMotorista(viagem.MotoristaViagem.ID, ref Resposta);
            ValidaVeiculo(viagem.VeiculoViagem.ID, ref Resposta);
        }
        private static void ValidaVinculo(int idMotorista, int idVeiculo, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            if (idMotorista == 0)
            {
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                Resposta.Mensagem += "Motorista não informado. \n";
            }
            if (idVeiculo == 0)
            {
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                Resposta.Mensagem += "Veículo não informado. \n";
            }
            if (Resposta.Status == EnumStatusResposta.Sucesso && !new VeiculoDAL().ListarVeiculosPorIDMotorista(idMotorista, true).Where(v => v.ID == idVeiculo).Any())
            {
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                Resposta.Mensagem += "O motorista não está vinculado ao veículo" + "\n";
            }
        }

        private static void ValidaMotorista(int idMotorista, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            if (idMotorista == 0)
            {
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                Resposta.Mensagem += "Motorista não informado. \n";
                return;
            }
            int[] statusViagem =
            {
                (int)ENUMSTATUSVIAGEM.EMANDAMENTO,
                (int)ENUMSTATUSVIAGEM.PROGRAMADA
            };
            var buscaMotorista = new ViagemDBE()
            {
                MotoristaViagem = new MotoristaDBE()
                {
                    ID = idMotorista
                },
            };
            foreach (var statusID in statusViagem)
            {
                buscaMotorista.ViagemStatus = new StatusViagemDBE()
                {
                    ID = statusID
                };
                var viagens = new ViagemDAL().Read(buscaMotorista);
                if (viagens.Any())
                {
                    Resposta.Status = EnumStatusResposta.ErroValidacao;
                    Resposta.Mensagem += "Motorista está vinculado a uma viagem em andamento: " + viagens.First().Codigo + "\n";
                }
            }
        }
        private static void ValidaVeiculo(int idVeiculo, ref RespostaNegocio<ViagemDBE> Resposta)
        {
            if (idVeiculo == 0)
            {
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                Resposta.Mensagem += "Veículo não informado. \n";
                return;
            }
            int[] statusViagem =
            {
                (int)ENUMSTATUSVIAGEM.EMANDAMENTO,
                (int)ENUMSTATUSVIAGEM.PROGRAMADA
            };
            var buscaVeiculo = new ViagemDBE()
            {
                VeiculoViagem = new VeiculoDBE()
                {
                    ID = idVeiculo
                }
            };
            foreach (var statusID in statusViagem)
            {
                buscaVeiculo.ViagemStatus = new StatusViagemDBE()
                {
                    ID = statusID
                };
                var viagens = new ViagemDAL().Read(buscaVeiculo);
                if (viagens.Any())
                {
                    Resposta.Status = EnumStatusResposta.ErroValidacao;
                    Resposta.Mensagem += "Veículo está vinculado a uma viagem em andamento: " + viagens.First().Codigo + "\n";
                }
            }
        }

        #endregion
    }

    public class DropDownMenuPainelViagens
    {
        public List<DropDownItem> Ordenacao { get; set; }
        public List<DropDownItem> Filtros { get; set; }
        public List<DropDownItem> CampoOrdenacao { get; set; }
        public List<DropDownItem> StatusViagem { get; set; }
    }
}
