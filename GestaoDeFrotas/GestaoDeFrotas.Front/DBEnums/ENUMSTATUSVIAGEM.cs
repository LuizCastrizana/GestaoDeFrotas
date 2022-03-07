using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CadastroDeCaminhoneiro.DBEnums
{
    [DataContract]
    public enum ENUMSTATUSVIAGEM
    {
        [EnumMember]
        PROGRAMADA = 1,

        [EnumMember]
        EMANDAMENTO = 2,

        [EnumMember]
        ENCERRADA = 3,

        [EnumMember]
        CANCELADA = 4,
    }
}