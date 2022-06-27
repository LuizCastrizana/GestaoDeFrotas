using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business
{
    [DataContract]
    public enum EnumStatusResposta
    {
        [EnumMember]
        Sucesso = 1,

        [EnumMember]
        Erro = 2,

        [EnumMember]
        ErroValidacao = 3,

        [EnumMember]
        Aviso = 4
    }
}
