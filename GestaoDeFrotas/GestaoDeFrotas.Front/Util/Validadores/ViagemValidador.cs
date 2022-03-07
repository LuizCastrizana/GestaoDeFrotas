using CadastroDeCaminhoneiro.DBEnums;
using CadastroDeCaminhoneiro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public static class ViagemValidador
    {
        public static RetornoValidacao ValidaInclusao(Viagem viagem)
        {
            var Retorno = new RetornoValidacao()
            {
                Sucesso = true,
                Mensagem = ""
            };

            ValidaVinculo(viagem.MotoristaViagem.ID, viagem.VeiculoViagem.ID, ref Retorno);
            if (!Retorno.Sucesso)
                return Retorno;
            ValidaMotorista(viagem.MotoristaViagem.ID, ref Retorno);
            ValidaVeiculo(viagem.VeiculoViagem.ID, ref Retorno);

            return Retorno;
        }
        private static void ValidaVinculo(int idMotorista, int idVeiculo, ref RetornoValidacao Retorno)
        {
            if (idMotorista == 0)
            {
                Retorno.Sucesso = false;
                Retorno.Mensagem += "Motorista não informado. \n";
            }
            if (idVeiculo == 0)
            {
                Retorno.Sucesso = false;
                Retorno.Mensagem += "Veículo não informado. \n";
            }
            if (Retorno.Sucesso && !new Veiculo().ListarVeiculosPorIDMotorista(idMotorista, true).Where(v => v.ID == idVeiculo).Any())
            {
                Retorno.Sucesso = false;
                Retorno.Mensagem += "O motorista não está vinculado ao veículo" + "\n";
            }
        }

        private static void ValidaMotorista(int idMotorista, ref RetornoValidacao Retorno)
        {
            if (idMotorista == 0)
            {
                Retorno.Sucesso = false;
                Retorno.Mensagem += "Motorista não informado. \n";
                return;
            }
            int[] statusViagem = 
            {
                (int)ENUMSTATUSVIAGEM.EMANDAMENTO,
                (int)ENUMSTATUSVIAGEM.PROGRAMADA
            };
            var buscaMotorista = new Viagem()
            {
                MotoristaViagem = new Motorista()
                {
                    ID = idMotorista
                },
            };
            foreach( var statusID in statusViagem)
            {
                buscaMotorista.ViagemStatus = new StatusViagem()
                {
                    ID = statusID
                };
                var viagens = buscaMotorista.Read();
                if (viagens.Any())
                {
                    Retorno.Sucesso = false;
                    Retorno.Mensagem += "Motorista está vinculado a uma viagem em andamento: " + viagens.First().Codigo + "\n";
                }
            }
        }
        private static void ValidaVeiculo(int idVeiculo, ref RetornoValidacao Retorno)
        {
            if (idVeiculo == 0)
            {
                Retorno.Sucesso = false;
                Retorno.Mensagem += "Veículo não informado. \n";
                return;
            }
            int[] statusViagem =
            {
                (int)ENUMSTATUSVIAGEM.EMANDAMENTO,
                (int)ENUMSTATUSVIAGEM.PROGRAMADA
            };
            var buscaVeiculo = new Viagem()
            {
                VeiculoViagem = new Veiculo()
                {
                    ID = idVeiculo
                }
            };
            foreach (var statusID in statusViagem)
            {
                buscaVeiculo.ViagemStatus = new StatusViagem()
                {
                    ID = statusID
                };
                var viagens = buscaVeiculo.Read();
                if (viagens.Any())
                {
                    Retorno.Sucesso = false;
                    Retorno.Mensagem += "Veículo está vinculado a uma viagem em andamento: " + viagens.First().Codigo + "\n";
                }
            }
            //var viagens = buscaVeiculo.Read(buscaVeiculo);
            //if (viagens.Any())
            //{
            //    Retorno.Sucesso = false;
            //    Retorno.Mensagem += "Veiculo está vinculado a uma viagem em andamento: " + viagens.First().Codigo + "\n";
            //}
        }
    }
}