using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GestaoDeFrotas.Data.Enums
{
    [DataContract]
    public enum ENUMSTATUS
    {
        [EnumMember]
        ATIVO = 1,
    }
}