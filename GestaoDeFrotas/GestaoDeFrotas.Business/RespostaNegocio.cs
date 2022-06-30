using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business
{
    public class RespostaNegocio<T>
    {
        public string Mensagem { get; set; }
        public EnumStatusResposta Status { get; set; }
        public T Retorno { get; set; } = default(T);

        public RespostaNegocio()
        {
            Status = EnumStatusResposta.Sucesso;
            Mensagem = string.Empty;
        }
    }
}
