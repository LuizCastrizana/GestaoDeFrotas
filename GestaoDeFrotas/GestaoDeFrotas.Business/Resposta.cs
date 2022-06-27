using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business
{
    public class Resposta<T>
    {
        public string Mensagem { get; set; }
        public EnumStatusResposta Status { get; set; }
        public T Retorno { get; set; } = default(T);

        public Resposta()
        {
            Status = EnumStatusResposta.Sucesso;
            Mensagem = string.Empty;
        }
    }
}
