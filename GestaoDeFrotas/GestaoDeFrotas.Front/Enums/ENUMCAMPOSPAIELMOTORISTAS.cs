using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GestaoDeFrotas.Enums
{
    [DataContract]
    public enum ENUMCAMPOSPAIELMOTORISTAS
    {
        [EnumMember]
        NOME = 1,

        [EnumMember]
        CPF = 2,

        [EnumMember]
        CNH = 3,

        [EnumMember]
        MUNICIPIO = 4,

        [EnumMember]
        DATAINCLUSAO = 5,

        [EnumMember]
        DATAALTERACAO = 6,

        [EnumMember]
        STATUS = 7,

    }
}