using LanguageConvertor.Components;
using LanguageConvertor.Core;
using LanguageConvertor.Utility;

namespace LanguageConvertor.Languages;

public sealed class CppBuilderConfig : FileBuilderConfig
{
    public CppBuilderConfig()
    {
        Language = ConvertibleLanguage.Cpp;

        NewStackAllocationFormat = (type) => $"{type}()";
        NewHeapAllocationFormat = (type) => $"new {type.Trim('*')}()";

        ParameterNameFormat = (name) => name.Replace("m_", "").ToLower();
        MemberInitializationFormat = (member, arg) => $"{member} = {arg};";
    }
}