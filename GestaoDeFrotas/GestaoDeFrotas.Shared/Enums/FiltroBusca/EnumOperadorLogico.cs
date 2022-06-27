using System.Runtime.Serialization;

namespace GestaoDeFrotas.Shared.Enums
{
    [DataContract]
    public enum EnumOperadorLogico
    {
        [EnumMember]
        E = 1,

        [EnumMember]
        OU = 2,

    }
}
