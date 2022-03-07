using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public class CadastroCNHException : Exception
    {
        public CadastroCNHException()
        {
        }
        public CadastroCNHException(string mensagem) : base(mensagem)
        {
        }
        public CadastroCNHException(string mensagem, Exception innerException) : base (mensagem, innerException)
        {
        }
    }
}