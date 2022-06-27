using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GestaoDeFrotas.Enums
{
    [DataContract]
    public enum ENUMCAMPOSPAIELVIAGEM
    {
        [EnumMember]
        CODIGO = 1,

        [EnumMember]
        DATAINCLUSAO = 2,

        [EnumMember]
        STATUS = 3,

        [EnumMember]
        VEICULO = 4,

        [EnumMember]
        MOTORISTA = 5,
    }
}