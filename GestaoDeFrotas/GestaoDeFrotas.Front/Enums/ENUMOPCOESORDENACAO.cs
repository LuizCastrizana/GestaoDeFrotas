using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CadastroDeCaminhoneiro.Enums
{
    [DataContract]
    public enum ENUMOPCOESORDENACAO
    {
        [EnumMember]
        CRESCENTE = 1,

        [EnumMember]
        DECRESCENTE = 2,
    }
}