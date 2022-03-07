using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas.Data
{
    public class CadastroEnderecoException : Exception
    {
        //Guarda o ID do endereço
        public uint ID { get; }
        public CadastroEnderecoException()
        {

        }
        public CadastroEnderecoException(string mensagem) : base(mensagem)
        {

        }
        public CadastroEnderecoException(string mensagem, Exception e) : base(mensagem, e)
        {

        }
        public CadastroEnderecoException(string mensagem, Exception e, uint id) : this(mensagem, e)
        {
            this.ID = id;
        }
    }
}