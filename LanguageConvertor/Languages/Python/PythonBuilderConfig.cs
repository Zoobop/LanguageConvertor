

using LanguageConvertor.Core;
using LanguageConvertor.Utility;

namespace LanguageConvertor.Languages;

public sealed class PythonBuilderConfig : FileBuilderConfig
{

	public PythonBuilderConfig()
	{
        Language = ConvertibleLanguage.Python;

        DefaultValueFormat = () => "()";
        NewStackAllocationFormat = (type) => type;
        NewHeapAllocationFormat = NewStackAllocationFormat;

        ConstructorNameFormat = (name) => "__init__";
        ParameterNameFormat = (name) => name.ToLower();
        MemberInitializationFormat = (member, arg) => $"self.{member} = {arg}";
    }

}
