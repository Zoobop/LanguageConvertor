using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal struct PropertyComponent : IComponent<PropertyComponent>
{
    public string? AccessModifier { get; set; }
    public string? SpecialModifier { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }

    public bool CanRead { get; }
    public bool CanWrite { get; }
    public string? WriteAccessModifer { get; set; }

    public bool IsConst { get => SpecialModifier == "const"; }
    public bool IsStatic { get => SpecialModifier == "static"; }
    public bool IsPrivate { get => AccessModifier == "private" || AccessModifier == null; }
    public bool IsPublic { get => AccessModifier == "public"; }
    public bool IsProtected { get => AccessModifier == "protected"; }
    public bool HasValue { get => string.IsNullOrEmpty(Value); }

    public PropertyComponent(string? accessModifier, string? specialModifier, string? type, string? name, string? value, bool canRead, bool canWrite, string? writeAccessModifier)
    {
        AccessModifier = accessModifier;
        SpecialModifier = specialModifier;
        Type = type;
        Name = name;
        Value = value;

        CanRead = canRead;
        CanWrite = canWrite;
        WriteAccessModifer = writeAccessModifier;
    }

    public static PropertyComponent Parse(string propertyLine)
    {
        var span = propertyLine.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var canRead = false;
        var canWrite = false;
        var writeAccessModifier = string.Empty;
        var value = string.Empty;

        // Try get accessor
        var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasAccess)
        {
            var length = span.IndexOf(' ');
            accessor = span[..length++].ToString();
            //Console.WriteLine($"[{accessor}]");
            span = span[length..];
        }

        // Try get special
        var hasSpecial = span.StartsWith("static") || span.StartsWith("const");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }

        // Get type
        var typeIndex = span.IndexOf(' ');
        var type = span[..typeIndex++].ToString();
        //Console.WriteLine($"[{type}]");
        span = span[typeIndex..];

        // Get name
        var tryIndex = span.IndexOf(' ');
        var nameIndex = (tryIndex == -1) ? span.IndexOf(';') : tryIndex;
        var name = span[..nameIndex++].ToString();
        //Console.WriteLine($"[{name}]");
        span = span[nameIndex..];

        // Try get getter
        var getterIndex = span.IndexOf('g');
        if (getterIndex != -1)
        {
            canRead = true;
            var nextIndex = getterIndex + 5;
            span = span[nextIndex..];
            //Console.WriteLine($"[{canRead}]");
        }

        // Try get write accessor
        var hasWriteAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasWriteAccess)
        {
            var length = span.IndexOf(' ');
            writeAccessModifier = span[..length++].ToString();
            //Console.WriteLine($"[{writeAccessModifier}]");
            span = span[length..];
        }

        // Try get setter
        var setterIndex = span.IndexOf('s');
        if (setterIndex != -1)
        {
            canWrite = true;
            //Console.WriteLine($"[{canWrite}]");
        }

        // Try get value
        var valueIndex = span.IndexOf('=');
        if (valueIndex != -1)
        {
            var index = valueIndex + 2; ;
            value = span[index..^1].TrimEnd(';').ToString();
            //Console.WriteLine($"[{value}]");
        }

        return new PropertyComponent(accessor, special, type, name, value, canRead, canWrite, writeAccessModifier);
    }

    public override string ToString()
    {
        return $"[{AccessModifier}] [{SpecialModifier}] [{Type}] [{Name}] [{CanRead}] [{CanWrite}] [{WriteAccessModifer}] [{Value}]";
    }
}
