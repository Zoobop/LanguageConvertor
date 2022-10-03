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

    public override string ToString()
    {
        return Name;
    }

    #region IComponent
    
    public bool IsScope() => true;
    
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
    
    #endregion
}
