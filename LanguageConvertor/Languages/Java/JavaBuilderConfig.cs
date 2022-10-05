using LanguageConvertor.Core;

namespace LanguageConvertor.Languages;

public sealed class JavaBuilderConfig : FileBuilderConfig
{

	public JavaBuilderConfig()
	{
        Language = ConvertibleLanguage.Java;

        DefaultValueFormat = () => "()";
        NewStackAllocationFormat = (type) => $"new {type.Trim('*')}";
        NewHeapAllocationFormat = NewStackAllocationFormat;

        ConstructorNameFormat = (name) => name;
        ParameterNameFormat = (name) => name.ToLower();
        MemberInitializationFormat = (member, arg) => $"this.{member} = {arg};";
    }
}
