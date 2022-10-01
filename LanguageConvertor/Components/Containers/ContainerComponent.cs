using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

public sealed class ContainerComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
    public bool IsFileScoped { get; init; } = false;
    public List<ContainerComponent> Containers { get; init; } = new List<ContainerComponent>();
    public List<ClassComponent> Classes { get; init; } = new List<ClassComponent>();
    public int Count => Classes.Count;

    public ContainerComponent()
    {
    }
    
    public ContainerComponent(string name, bool isFileScoped)
    {
        Name = name;
        IsFileScoped = isFileScoped;
    }

    public ContainerComponent(string name, bool isFileScoped, List<ClassComponent> classes)
    {
        Name = name;
        IsFileScoped = isFileScoped;
        Classes = classes;
    }

    public void AddClass(in ClassComponent classComponent)
    {
        Classes.Add(classComponent);
    }

    /*public static ContainerComponent Parse(string containerData)
    {
        var span = containerData.AsSpan();
        span = span.Trim();

        var name = string.Empty;
        var isFileScoped = false;

        // Skip 'namespace' keyword
        var namespaceIndex = span.IndexOf(' ');
        span = span[++namespaceIndex..];

        // Get name
        var tryNameIndex = span.IndexOf(';');
        if (tryNameIndex == -1)
        {
            name = span.ToString();
        }
        else
        {
            name = span[..^1].ToString();
            isFileScoped = true;
        }

        return new ContainerComponent(name, isFileScoped);
    }*/

    public override string ToString()
    {
        return Name;
    }

    public void AddComponent(in IComponent component)
    {
        var type = component.GetType();
        if (type == typeof(ContainerComponent))
        {
            Containers.Add((ContainerComponent)component);
        }
        else
        {
            Classes.Add((ClassComponent)component);
        }
    }
}
