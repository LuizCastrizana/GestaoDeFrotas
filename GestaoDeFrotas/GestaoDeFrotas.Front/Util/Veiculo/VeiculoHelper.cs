using CadastroDeCaminhoneiro.Enums;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public static class VeiculoHelper
    {
        public static IEnumerable<VeiculoDBE> BuscarVeiculosPainel(string busca, int opcaoOrdenacao, int opcaoFiltragem, bool? todos)
        {
            var lista = new VeiculoDAL().ListarVeiculos(todos)
                    .Where(m => StringTools.RemoverCaracteres(m.Placa, "-")
                    .Contains(StringTools.RemoverCaracteres(busca, "-")));

            if (opcaoOrdenacao == (int)ENUMOPCOESORDENACAO.CRESCENTE)
            {
                switch (opcaoFiltragem)
                {
                    case (int)ENUMCAMPOSPAIELVEICULOS.PLACA:
                        lista = lista.OrderBy(m => m.Placa);
                        break;
                    case (int)ENUMCAMPOSPAIELVEICULOS.DATAINCLUSAO:
                        lista = lista.OrderBy(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAIELVEICULOS.STATUS:
                        lista = lista.OrderBy(m => m.Status);
                        break;
                    default:
                        lista = lista.OrderBy(m => m.Placa);
                        break;
                }
            }
            else
            {
                switch (opcaoFiltragem)
                {
                    case (int)ENUMCAMPOSPAIELVEICULOS.PLACA:
                        lista = lista.OrderByDescending(m => m.Placa);
                        break;
                    case (int)ENUMCAMPOSPAIELVEICULOS.DATAINCLUSAO:
                        lista = lista.OrderByDescending(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAIELVEICULOS.STATUS:
                        lista = lista.OrderByDescending(m => m.Status);
                        break;
                    default:
                        lista = lista.OrderByDescending(m => m.Placa);
                        break;
                }
            }

            return lista;
        }
    }
}