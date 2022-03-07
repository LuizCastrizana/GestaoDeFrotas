using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public static class StringTools
    {
        /// <summary>
        /// Método estático que retorna uma data criada a partir da string e cultura passados como parâmetros
        /// </summary>
        /// <param name="data">Data em formato string</param>
        /// <param name="nomeCultura">"Nome da cultura do formato da data. Exemplo: en-US"</param>
        /// <returns></returns>
        public static DateTime ConverterEmData(string data, string nomeCultura)
        {
            if (nomeCultura == null || nomeCultura == string.Empty)
                nomeCultura = "en-US";

            CultureInfo cultura = new CultureInfo(nomeCultura);

            DateTime retorno = Convert.ToDateTime(data, cultura);

            return retorno;
        }

        /// <summary>
        /// Remove da string de entrada os caracteres presentes na string de parâmetro
        /// </summary>
        /// <param name="caracteresARemover">String contendo os caracteres a serem removidos da entrada.</param>
        /// <param name="entrada">String de entrada, da qual serão removidos os caracteres.</param>
        /// <returns>String de entrada sem os caracteres informados</returns>
        public static string RemoverCaracteres(string entrada, string caracteresARemover)
        {
            for (int i = 0; i < caracteresARemover.Length; i++)
            {
                var caractere = caracteresARemover.Substring(i, 1);
                entrada = entrada.Replace(caractere, string.Empty);
            }
            return entrada;
        }
    }
}