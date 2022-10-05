

using System.Xml.Linq;

namespace LanguageConvertor.Utility;

internal static class StringExtensionMethods
{
    public static string ToCamelCase(this string original)
    {
        return string.Empty;
    }

    public static string ToPascalCase(this string original)
    {
        var span = original.AsSpan();
        var newString = span[0..1];

        return $"{newString.ToString().ToLower()}{span[1..].ToString()}";
    }

}
