using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas
{
    public static class VeiculoTools
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
                    case (int)ENUMCAMPOSPAINELVEICULOS.PLACA:
                        lista = lista.OrderBy(m => m.Placa);
                        break;
                    case (int)ENUMCAMPOSPAINELVEICULOS.DATAINCLUSAO:
                        lista = lista.OrderBy(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAINELVEICULOS.STATUS:
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
                    case (int)ENUMCAMPOSPAINELVEICULOS.PLACA:
                        lista = lista.OrderByDescending(m => m.Placa);
                        break;
                    case (int)ENUMCAMPOSPAINELVEICULOS.DATAINCLUSAO:
                        lista = lista.OrderByDescending(m => m.DataInclusao);
                        break;
                    case (int)ENUMCAMPOSPAINELVEICULOS.STATUS:
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