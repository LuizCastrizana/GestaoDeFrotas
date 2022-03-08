using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public static class MotoristaHelper
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
    }
}