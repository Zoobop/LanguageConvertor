using LanguageConvertor.Components;
using LanguageConvertor.Core;
using LanguageConvertor.Utility;

namespace LanguageConvertor.Languages;

public sealed class CppBuilderConfig : FileBuilderConfig
{
    private static readonly IEnumerable<string> _primitiveTypes = new HashSet<string>
    {
        "bool", "char", "int8_t", "int16_t", "int32_t",
        "int64_t", "uint8_t", "uint16_t", "uint32_t", "uint64_t",
    };

    public CppBuilderConfig()
    {
        Language = ConvertibleLanguage.Cpp;

        DefaultValueFormat = () => "()";
        NewStackAllocationFormat = (type) => $"{type}";
        NewHeapAllocationFormat = (type) => $"new {type.Trim('*')}";

        ConstructorNameFormat = (name) => name;
        ParameterNameFormat = (name) => name.Replace("m_", "").ToLower();
        MemberInitializationFormat = (member, arg) => $"{member} = {arg};";
    }
}