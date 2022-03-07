using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CadastroDeCaminhoneiro.Enums
{
    [DataContract]
    public enum ENUMCAMPOSPAIELVEICULOS
    {
        [EnumMember]
        PLACA = 1,

        [EnumMember]
        DATAINCLUSAO = 2,

        [EnumMember]
        STATUS = 3,

    }
}