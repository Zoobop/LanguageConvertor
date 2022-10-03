using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

public sealed class FieldComponent : IComponent
{
    public string AccessModifier { get; set; } = string.Empty;
    public string SpecialModifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public bool IsPointer => Type.EndsWith('*');
    public bool IsConst => SpecialModifier == "const";
    public bool IsStatic => SpecialModifier == "static";
    public bool IsPrivate => AccessModifier is "private" or null;
    public bool IsPublic => AccessModifier == "public";
    public bool IsProtected => AccessModifier == "protected";
    public bool HasValue => string.IsNullOrEmpty(Value);

    public FieldComponent()
    {
    }

    public FieldComponent(string accessModifier, string specialModifier, string type, string name, string value)
    {
        AccessModifier = accessModifier;
        SpecialModifier = specialModifier;
        Type = type;
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return Name;
    }

    #region IComponent

    public bool IsScope() => false;

    #endregion
}
