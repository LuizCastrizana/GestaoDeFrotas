using System.Runtime.Serialization;

namespace GestaoDeFrotas.Shared.Enums
{
    [DataContract]
    public enum EnumTipoCondicao
    {
        [EnumMember]
        IGUAL = 1,

        [EnumMember]
        MAIOR_OU_IGUAL = 2,

        [EnumMember]
        MENOR_OU_IGUAL = 3,

        [EnumMember]
        DIFERENTE = 4, 
        
        [EnumMember]
        CONTEM = 5,
    }
}
