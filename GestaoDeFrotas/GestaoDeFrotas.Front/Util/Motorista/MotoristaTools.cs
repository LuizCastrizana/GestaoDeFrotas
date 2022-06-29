using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas
{
    public static class MotoristaTools
    {
        public static IEnumerable<MotoristaDBE> BuscarPorNomeOuCPF (string busca, bool? todos)
        {
            if (busca == null)
                busca = string.Empty;
            var lista = new MotoristaDAL().List(todos).
                Where(m => (m.PrimeiroNome + " " + m.Sobrenome).ToUpper().Contains(busca.ToUpper())
                ||
                (StringTools.RemoverCaracteres(m.CPF, "-.")).Contains(StringTools.RemoverCaracteres(busca, "-.")));

            return lista;
        }

        public static OpcoesPainelMotorista MontarListasOpcoesPainel()
        {
            var retorno = new OpcoesPainelMotorista
            {
                Ordenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                },
                Filtros = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO, "Município"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO, "Data inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO, "Data alteração"),
                },
                CampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO, "Município"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO, "Data inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO, "Data alteração"),
                },
            };

            return retorno;
        }
    }
}