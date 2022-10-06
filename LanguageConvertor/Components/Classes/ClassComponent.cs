using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

public sealed class ClassComponent : IComponent
{
    public string AccessModifier { get; set; } = string.Empty;
    public string ClassType { get; set; } = string.Empty;
    public string SpecialModifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ParentClass { get; set; } = string.Empty;
    public List<string> Interfaces { get; } = new List<string>();
    public List<ClassComponent> Classes { get; } = new List<ClassComponent>();
    public List<FieldComponent> Fields { get; } = new List<FieldComponent>();
    public List<PropertyComponent> Properties { get; } = new List<PropertyComponent>();
    public List<MethodComponent> Methods { get; } = new List<MethodComponent>();

    public bool IsStatic { get => SpecialModifier == "static"; }
    public bool IsPrivate { get => AccessModifier is "private" or null; }
    public bool IsPublic { get => AccessModifier == "public"; }
    public bool IsProtected { get => AccessModifier == "protected"; }
    public bool IsInterface { get => ClassType == "interface"; }
    public bool IsStruct { get => ClassType == "struct"; }
    public bool IsEnum { get => ClassType == "enum"; }

    public ClassComponent()
    {
    }

    public ClassComponent(string accessModifier, string specialModifier, string name, string parentClass, params string[] interfaces)
    {
        AccessModifier = accessModifier;
        SpecialModifier = specialModifier;
        Name = name;
        ParentClass = parentClass;
        Interfaces = new List<string>(interfaces);
    }
    
    public ClassComponent(string accessModifier, string type, string specialModifier, string name, string parentClass, List<string> interfaces)
    {
        AccessModifier = accessModifier;
        ClassType = type;
        SpecialModifier = specialModifier;
        Name = name;
        ParentClass = parentClass;
        Interfaces = interfaces;
    }

    public void AddClass(in ClassComponent @class)
    {
        Classes.Add(@class);
    }

    public void AddField(in FieldComponent field)
    {
        Fields.Add(field);
    }

    public void AddProperty(in PropertyComponent property)
    {
        Properties.Add(property);
    }

    public void AddMethod(in MethodComponent method)
    {
        Methods.Add(method);
    }

    public override string ToString()
    {
        return Name;
    }

    #region IComponent

    public bool IsScope() => true;
    
    public void AddComponent(in IComponent component)
    {
        var type = component.GetType();
        if (type == typeof(ClassComponent))
        {
            var @class = (ClassComponent)component;
            Classes.Add(@class);
        }
        else if (type == typeof(MethodComponent))
        {
            var method = (MethodComponent)component;
            Methods.Add(method);
        }
        else if (type == typeof(PropertyComponent))
        {
            var property = (PropertyComponent)component;
            Properties.Add(property);
        }
        else
        {
            var field = (FieldComponent)component;
            Fields.Add(field);
        }
    }
    
    #endregion
}
