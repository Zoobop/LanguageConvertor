using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal sealed class ClassComponent : IComponent
{
    public string? AccessModifier { get; set; }
    public string? SpecialModifier { get; set; }
    public string Name { get; set; }
    public string? ParentClass { get; set; }
    public List<string> Interfaces { get; } = new List<string>();
    public List<ClassComponent> Classes { get; } = new List<ClassComponent>();
    public List<FieldComponent> Fields { get; } = new List<FieldComponent>();
    public List<PropertyComponent> Properties { get; } = new List<PropertyComponent>();
    public List<MethodComponent> Methods { get; } = new List<MethodComponent>();

    public bool IsStatic { get => SpecialModifier == "static"; }
    public bool IsPrivate { get => AccessModifier == "private" || AccessModifier == null; }
    public bool IsPublic { get => AccessModifier == "public"; }
    public bool IsProtected { get => AccessModifier == "protected"; }

    public ClassComponent(string? accessModifier, string? specialModifier, string name, string? parentClass, List<string> interfaces)
    {
        AccessModifier = accessModifier;
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

    public static ClassComponent Parse(string classData)
    {
        var span = classData.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var name = string.Empty;
        var parent = string.Empty;
        var interfaces = new List<string>();

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
        var hasSpecial = span.StartsWith("static") || span.StartsWith("sealed") || span.StartsWith("virtual") || span.StartsWith("abstract");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }

        // Skip 'class' keyword
        var classIndex = span.IndexOf(' ');
        span = span[++classIndex..];

        // Get name
        var tryNameIndex = span.IndexOf(' ');
        if (tryNameIndex == -1)
        {
            name = span.ToString();
        }
        else
        {
            name = span[..tryNameIndex++].ToString();
            //Console.WriteLine($"[{name}]");
            span = span[tryNameIndex..];
        }

        // Try get parents
        var baseIndex = span.IndexOf(':');
        if (baseIndex != -1)
        {
            var startIndex = baseIndex + 2;
            span = span[startIndex..].Trim();

            var hasParents = true;
            while (hasParents)
            {
                // Get parent
                var tryParentIndex = span.IndexOf(',');
                var currentParent = string.Empty;
                if (tryParentIndex == -1)
                {
                    // Last one
                    currentParent = span.ToString();
                }
                else
                {
                    currentParent = span[..tryParentIndex++].ToString();
                    span = span[++tryParentIndex..];
                }

                //Console.WriteLine($"[{currentParent}]");

                // Interface check
                if (currentParent.StartsWith('I'))
                {
                    // Interface
                    interfaces.Add(currentParent);
                }
                else
                {
                    // Class
                    parent = currentParent;
                }

                // Break
                if (tryParentIndex == -1) hasParents = false;
            }
        }

        return new ClassComponent(accessor, special, name, parent, interfaces);
    }

    public string Definition()
    {
        return $"[{AccessModifier}] [{SpecialModifier}] [{Name}] [{ParentClass}] [{(Interfaces != null ? string.Join(", ", Interfaces) : string.Empty)}]";
    }

    public override string ToString()
    {
        return Name;
    }

    public bool IsScope()
    {
        return true;
    }

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
}
