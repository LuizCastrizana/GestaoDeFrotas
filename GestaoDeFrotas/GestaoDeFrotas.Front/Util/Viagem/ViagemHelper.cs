using CadastroDeCaminhoneiro.Enums;
using CadastroDeCaminhoneiro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{ 
    public static class ViagemHelper
    {
        public static IEnumerable<Viagem> BuscarViagensPainel(Viagem ViagemBusca, string busca, int opcaoFiltragem, int opcaoCampoOrdenacao, int opcaoOrdenacao)
        {
            var lista = Enumerable.Empty<Viagem>();
            if (busca == null)
                busca = string.Empty;

            switch (opcaoFiltragem)
            {
                case (int)ENUMCAMPOSPAIELVIAGEM.CODIGO:
                    lista = ViagemBusca.Read().Where(m => m.Codigo.Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.DATAINCLUSAO:
                    lista = ViagemBusca.Read().Where(m => m.DataInclusao.ToString("dd/MM/yyyy").Contains(busca));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.STATUS:
                    lista = ViagemBusca.Read().Where(m => m.ViagemStatus.ID.ToString().Contains(busca.ToUpper()));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.VEICULO:
                    lista = ViagemBusca.Read().Where(m => StringTools.RemoverCaracteres(m.VeiculoViagem.Placa.ToUpper(), "-").Contains(StringTools.RemoverCaracteres(busca.ToUpper(), "-")));
                    break;
                case (int)ENUMCAMPOSPAIELVIAGEM.MOTORISTA:
                    lista = ViagemBusca.Read().
                        Where(m => (m.MotoristaViagem.PrimeiroNome + " " + m.MotoristaViagem.Sobrenome).ToUpper().Contains(busca.ToUpper())
                        ||
                        (StringTools.RemoverCaracteres(m.MotoristaViagem.CPF, "-.")).Contains(StringTools.RemoverCaracteres(busca, "-.")));
                    break;
                default:
                    lista = ViagemBusca.List(false);
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
    }
}