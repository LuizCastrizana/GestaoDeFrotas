using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GestaoDeFrotas.Data.Enums
{
    [DataContract]
    public enum ENUMMOTIVOVIAGEM
    {
        [EnumMember]
        SERVICOEXTERO = 1,

        [EnumMember]
        MANUTENCAO = 2,

        [EnumMember]
        DESLOCAMENTO = 3,
    }
}