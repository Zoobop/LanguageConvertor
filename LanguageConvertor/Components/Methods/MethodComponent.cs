using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

public sealed class MethodComponent : IComponent
{
    public string AccessModifier { get; set; } = string.Empty;
    public string SpecialModifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<ParameterPack> Parameters { get; init; } = new List<ParameterPack>();
    public List<string> Body { get; init; } = new List<string>();

    public bool IsStatic { get => SpecialModifier == "static"; }
    public bool IsPrivate { get => AccessModifier is "private" or null; }
    public bool IsPublic { get => AccessModifier == "public"; }
    public bool IsProtected { get => AccessModifier == "protected"; }
    public bool IsAbstract { get => SpecialModifier == "abstract" || IsInterfaceAbstract; }
    public bool IsInterfaceAbstract { get; set; } = false;
    public bool IsOverride { get => SpecialModifier == "override"; }
    public bool IsConstructor { get => string.IsNullOrEmpty(Type); }
    public bool IsVoid { get => Type == "void"; }
    public bool HasParameters { get => Parameters is {Count: > 0}; }

    public MethodComponent()
    {
    }

    public MethodComponent(string accessModifier, string specialModifier, string type, string name, List<ParameterPack> parameters)
    {
        AccessModifier = accessModifier;
        SpecialModifier = specialModifier;
        Type = type;
        Name = name;
        Parameters = parameters;
    }

    public MethodComponent(string accessModifier, string specialModifier, string type, string name, params ParameterPack[] parameters)
    {
        AccessModifier = accessModifier;
        SpecialModifier = specialModifier;
        Type = type;
        Name = name;
        Parameters.AddRange(parameters);
    }

    #region Utility

    public void AddToBody(string line)
    {
        Body.Add(line);
    }

    public void AddToBody(IEnumerable<string> lines)
    {
        Body.AddRange(lines);
    }

    public void AddParameter(in ParameterPack parameter)
    {
        Parameters.Add(parameter);
    }

    public void AddParameters(params ParameterPack[] parameters)
    {
        Parameters.AddRange(parameters);
    }

    #endregion

    public override string ToString()
    {
        return Name;
    }

    #region IComponent
    
    public bool IsScope() => true;

    #endregion
}
