using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal struct ContainerComponent
{
    public string? Name { get; set; }
    public bool IsFileScoped { get; }

    public ContainerComponent(string? name, bool isFileScoped)
    {
        Name = name;
        IsFileScoped = isFileScoped;
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
}
