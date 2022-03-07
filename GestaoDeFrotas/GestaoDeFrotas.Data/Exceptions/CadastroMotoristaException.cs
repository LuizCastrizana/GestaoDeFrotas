using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas.Data
{
    public class CadastroMotoristaException : Exception
    {
        public CadastroMotoristaException()
        {

        }
        public CadastroMotoristaException(string mensagem) : base(mensagem)
        {

        }
        public CadastroMotoristaException(string mensagem, Exception e) : base(mensagem, e)
        {

        }
    }
}