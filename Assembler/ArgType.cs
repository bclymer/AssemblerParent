using System.Runtime.Serialization;

namespace Assembler
{
    public enum ArgType
    {
        [EnumMember]
        HasDollarSign,
        [EnumMember]
        HasParaenthesis,
        [EnumMember]
        JustAValue
    }
}
