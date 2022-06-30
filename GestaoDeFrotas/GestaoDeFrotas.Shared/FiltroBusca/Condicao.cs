using GestaoDeFrotas.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Shared.FiltroBusca
{
    public class Condicao
    {
        public string NomeCampo { get; set; }
        public string Valor { get; set; }
        public EnumTipoCondicao TipoCondicao { get; set; }
        public EnumTipoCampo TipoCampo { get; set; }

        public Condicao()
        {
            Valor = String.Empty;
        }

        public Condicao(string nome, DateTime valor, EnumTipoCondicao tipoCondicao)
        {
            NomeCampo = nome;
            Valor = ConverterData(valor);
            TipoCampo = EnumTipoCampo.DATA;
            TipoCondicao = tipoCondicao;
        }

        public Condicao(string nome, bool valor, EnumTipoCondicao tipoCondicao)
        {
            NomeCampo = nome;
            Valor = valor ? "1" : "0";
            TipoCampo = EnumTipoCampo.BOOLEANO;
            TipoCondicao = tipoCondicao;
        }
        public Condicao(string nome, string valor, EnumTipoCampo tipoCampo, EnumTipoCondicao tipoCondicao)
        {
            NomeCampo = nome;
            Valor = valor;
            TipoCampo = tipoCampo;
            TipoCondicao = tipoCondicao;
        }

        /// <summary>
        /// Converte uma variavel DateTime em uma string de data no formato do Oracle pt-BR
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ConverterData(DateTime data)
        {
            var retorno = data.ToString("dd/MM/yyyy");
            return retorno;
        }
    }
}
