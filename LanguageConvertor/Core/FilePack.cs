using LanguageConvertor.Components;

namespace LanguageConvertor.Core;

public readonly struct FilePack
{
    public ConvertibleLanguage Language { get; } = ConvertibleLanguage.Java;
    public List<ImportComponent> Imports { get; } = new List<ImportComponent>();
    public List<ContainerComponent> Containers { get; } = new List<ContainerComponent>();
    public List<ClassComponent> Classes { get; } = new List<ClassComponent>();
    public List<MethodComponent> Methods { get; } = new List<MethodComponent>();
    public List<FieldComponent> Fields { get; } = new List<FieldComponent>();
    public List<PropertyComponent> Properties { get; } = new List<PropertyComponent>();
    public ISet<IComponent> Components { get; } = new HashSet<IComponent>();
    public int TotalCount { get => Components.Count; }

    public FilePack(ConvertibleLanguage language)
    {
        Language = language;
    }
    
    public FilePack(
        ConvertibleLanguage language,
        List<ImportComponent> imports, 
        List<ContainerComponent> containers, 
        List<ClassComponent> classes, 
        List<MethodComponent> methods, 
        List<FieldComponent> fields, 
        List<PropertyComponent> properties)
    {
        Language = language;

        Imports.AddRange(imports);
        Containers.AddRange(containers);
        Classes.AddRange(classes);
        Methods.AddRange(methods);
        Fields.AddRange(fields);
        Properties.AddRange(properties);

        // Get all components
        foreach (var container in containers)
        {
            Components.Add(container);
        }
        foreach (var @class in classes)
        {
            Components.Add(@class);
        }
        foreach (var method in methods)
        {
            Components.Add(method);
        }
        foreach (var field in fields)
        {
            Components.Add(field);
        }
        foreach (var property in properties)
        {
            Components.Add(property);
        }
    }

    public void AddImport(in ImportComponent importComponent)
    {
        Imports.Add(importComponent);
    }

    public void AddContainer(in ContainerComponent containerComponent)
    {
        Containers.Add(containerComponent);
    }

    public void AddClass(in ClassComponent classComponent)
    {
        Classes.Add(classComponent);
    }

    public void AddMethod(in MethodComponent methodComponent)
    {
        Methods.Add(methodComponent);
    }

    public void AddField(in FieldComponent fieldComponent)
    {
        Fields.Add(fieldComponent);
    }

    public void AddProperty(in PropertyComponent propertyComponent)
    {
        Properties.Add(propertyComponent);
    }

    public void AddComponent(IComponent component)
    {
        Components.Add(component);
    }

    public void RemoveComponent(IComponent component)
    {
        Components.Remove(component);
    }
}
