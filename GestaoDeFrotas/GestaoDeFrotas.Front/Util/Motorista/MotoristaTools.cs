using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Enums;
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
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.DATAALTERACAO, "Data Alteração"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.MUNICIPIO, "Municipio"),
                },
                CampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.DATAINCLUSAO, "Data Inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.DATAALTERACAO, "Data Alteração"),
                    new DropDownItem((int)ENUMCAMPOSPAIELMOTORISTAS.MUNICIPIO, "Municipio"),
                },
            };

            return retorno;
        }
    }
}