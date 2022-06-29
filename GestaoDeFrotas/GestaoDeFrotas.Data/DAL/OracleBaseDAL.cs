using GestaoDeFrotas.Shared.Enums;
using GestaoDeFrotas.Shared.FiltroBusca;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DAL
{
    public class OracleBaseDAL
    {
        protected readonly string stringConexao = ConfigurationManager.AppSettings["StringOracle"];
        protected OracleConnection Conexao { get; set; }
        protected OracleCommand Comando { get; set; }
        protected OracleDataReader DataReader { get; set; }

        public string MontarFiltros(FiltroBusca filtro)
        {
            if (filtro.ListaExpressao == null)
                return string.Empty;

            StringBuilder retorno = new StringBuilder(" AND ");
            retorno.Append("(");

            for (int i = 0; i < filtro.ListaExpressao.Count; i++)
            {
                if (i > 0)
                {
                    if (filtro.ListaExpressao[i].OperadorLogico == EnumOperadorLogico.E)
                        retorno.Append(" AND ");
                    else
                        retorno.Append(" OR ");
                }

                retorno.Append("(");

                for (int j = 0; j < filtro.ListaExpressao[i].ListaCondicao.Count; j++)
                {
                    if (j > 0)
                    {
                        if (filtro.ListaExpressao[i].OperadorLogico == EnumOperadorLogico.E)
                            retorno.Append(" AND ");
                        else
                            retorno.Append(" OR ");
                    }
                    
                    retorno.Append("(");
                    retorno.Append(filtro.ListaExpressao[i].ListaCondicao[j].NomeCampo);

                    switch (filtro.ListaExpressao[i].ListaCondicao[j].TipoCondicao)
                    {
                        case EnumTipoCondicao.IGUAL:
                            retorno.Append(" = ");
                            break;
                        case EnumTipoCondicao.DIFERENTE:
                            retorno.Append(" != ");
                            break;
                        case EnumTipoCondicao.MAIOR_OU_IGUAL:
                            retorno.Append(" >= ");
                            break;
                        case EnumTipoCondicao.MENOR_OU_IGUAL:
                            retorno.Append(" <= ");
                            break;
                        case EnumTipoCondicao.CONTEM:
                            retorno.Append(" LIKE ");
                            break;
                    }

                    switch (filtro.ListaExpressao[i].ListaCondicao[j].TipoCampo)
                    {
                        case EnumTipoCampo.NUMERICO:
                        case EnumTipoCampo.BOOLEANO:
                            retorno.Append(filtro.ListaExpressao[i].ListaCondicao[j].Valor);
                            break;
                        case EnumTipoCampo.TEXTO:
                            if (filtro.ListaExpressao[i].ListaCondicao[j].TipoCondicao == EnumTipoCondicao.CONTEM)
                                retorno.Append(" \'%" + filtro.ListaExpressao[i].ListaCondicao[j].Valor.ToUpper() + "%\' ");
                            else
                                retorno.Append(" \'" + filtro.ListaExpressao[i].ListaCondicao[j].Valor.ToUpper() + "\' ");
                            break;
                        case EnumTipoCampo.DATA:
                            retorno.Append(" TO_DATE(\'" + filtro.ListaExpressao[i].ListaCondicao[j].Valor + "\', \'DD/MM/YYYY\') ");
                            break;
                    }
                    retorno.Append(")");
                }
                retorno.Append(")");
            }
            retorno.Append(")");

            return retorno.ToString();
        }
    }
}
