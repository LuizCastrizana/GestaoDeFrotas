using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Shared.Enums
{
    [DataContract]
    public enum EnumTipoCampo
    {
        [EnumMember]
        NUMERICO = 1,

        [EnumMember]
        TEXTO = 2,

        [EnumMember]
        BOOLEANO = 3,

        [EnumMember]
        DATA = 4,
    }
}
