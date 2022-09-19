using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal sealed class ContainerComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
    public bool IsFileScoped { get; } = false;
    public List<ClassComponent> Classes { get; } = new List<ClassComponent>();
    public int Count { get => Classes.Count; }

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

    public void AddClass(in ClassComponent @class)
    {
        Classes.Add(@class);
    }

    public static ContainerComponent Parse(string containerData)
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
    }

    public override string ToString()
    {
        return $"[{Name}] [{IsFileScoped}]";
    }

    public bool IsScope()
    {
        return true;
    }
}
