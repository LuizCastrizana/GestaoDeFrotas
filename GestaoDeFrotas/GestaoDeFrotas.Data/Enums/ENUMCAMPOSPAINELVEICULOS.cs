using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GestaoDeFrotas.Data.Enums
{
    [DataContract]
    public enum ENUMCAMPOSPAINELVEICULOS
    {
        [EnumMember]
        PLACA = 1,

        [EnumMember]
        DATAINCLUSAO = 2,

        [EnumMember]
        STATUS = 3,

    }
}