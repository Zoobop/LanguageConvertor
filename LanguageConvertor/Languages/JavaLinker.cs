using LanguageConvertor.Components;
using LanguageConvertor.Core;
using System;
using System.Text;

namespace LanguageConvertor.Languages;

internal class JavaLinker : Linker
{
    public JavaLinker(IEnumerable<string> data) : base(data)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Java;
    }

    protected override string GetImportKeyword()
    {
        return "import";
    }

    protected override string GetContainerKeyword()
    {
        return "package";
    }

    protected override string FormatImport(string importName)
    {
        return $"{GetImportKeyword()} {importName};";
    }

    protected override string FormatContainer(in ContainerComponent containerComponent)
    {
        // Format container
        var format = new StringBuilder();

        // Add container keyword
        format.Append($"{GetContainerKeyword()} ");

        // Add name
        var name = containerComponent.Name;
        format.Append(name);

        // Try add file-scoped indicator
        var isFileScoped = containerComponent.IsFileScoped;
        if (isFileScoped)
        {
            format.Append(';');
        }

        return format.ToString();
    }

    protected override string FormatClass(in ClassComponent classComponent)
    {
        // Format class definition
        var format = new StringBuilder();

        // Try add accessor
        var accessor = classComponent.AccessModifier;
        if (!string.IsNullOrEmpty(accessor))
        {
            format.Append($"{accessor} ");
        }

        // Try add special
        var special = classComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special))
        {
            format.Append($"{special} ");
        }

        // Add 'class' keyword
        format.Append("class ");

        // Add name
        var name = classComponent.Name;
        format.Append(name);

        // Try add parent class
        var parentClass = classComponent.ParentClass;
        if (!string.IsNullOrEmpty(parentClass))
        {
            format.Append($" extends {parentClass}");
        }

        // Try add interface(s)
        var interfaces = classComponent.Interfaces;
        if (interfaces.Count > 0)
        {
            format.Append($" implements {string.Join(", ", interfaces)}");
        }

        return format.ToString();
    }

    protected override string FormatMethod(in MethodComponent methodComponent)
    {
        // Format method definition
        var format = new StringBuilder();

        // Try add accessor
        var accessor = methodComponent.AccessModifier;
        if (!string.IsNullOrEmpty(accessor))
        {
            format.Append($"{accessor} ");
        }

        // Try add special
        var special = methodComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special))
        {
            format.Append($"{special} ");
        }

        // Add return type
        var returnType = methodComponent.Type;
        format.Append($"{returnType} ");

        // Add name
        var name = methodComponent.Name;
        format.Append(name);

        // Try add parameters
        var hasParameters = methodComponent.HasParameters;
        format.Append('(');
        if (hasParameters)
        {
            // Format each parameter
            foreach (var (argName, argType) in methodComponent.Parameters)
            {
                format.Append($"{argType} {argName},");
            }

            // Remove last comma
            format.Remove(format.Length - 1, 1);
        }
        format.Append(')');

        return format.ToString();
    }

    protected override string FormatProperty(in PropertyComponent propertyComponent)
    {
        throw new NotImplementedException();
    }

    protected override string FormatField(in FieldComponent fieldComponent)
    {
        throw new NotImplementedException();
    }

    protected override void ConstructMethods(List<string> file, List<string> methods)
    {
        throw new NotImplementedException();
    }

    protected override void ConstructMembers(List<string> file, List<string> members)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetFormattedFileData()
    {
        // AGGREGATE
        var allComponents = new List<IComponent>(_parser.TotalCount);
        allComponents.AddRange(_parser.Containers.ConvertAll<IComponent>(x => x));
        allComponents.AddRange(_parser.Classes.ConvertAll<IComponent>(x => x));
        allComponents.AddRange(_parser.Methods.ConvertAll<IComponent>(x => x));
        allComponents.AddRange(_parser.Properties.ConvertAll<IComponent>(x => x));
        allComponents.AddRange(_parser.Fields.ConvertAll<IComponent>(x => x));

        // FORMATTING
        var format = new List<string>(allComponents.Count);

        // Imports
        foreach (var import in _parser.Imports)
        {
            Append(FormatImport(import));
        }
        Append();

        var componentOrder = _parser.ComponentOrder;
        while (componentOrder.TryDequeue(out var component))
        {
            // Find the matching component
            var index = allComponents.FindIndex(x => ReferenceEquals(x, component.Component));
            if (index != -1)
            {
                // Format componnent
                switch (component.Type)
                {
                    case ComponentType.Container:
                        // Get formatted data
                        var containerComponent = (ContainerComponent)allComponents[index];

                        BuildContainer(containerComponent);
                        break;
                    case ComponentType.Class:
                        // Get formatted data
                        var classComponent = (ClassComponent)allComponents[index];

                        BuildClass(classComponent);
                        break;
                    case ComponentType.Method:
                        // Get formatted data
                        var methodComponent = (MethodComponent)allComponents[index];

                        BuildMethod(methodComponent);
                        break;
                    case ComponentType.Property:
                        // Get formatted data
                        var propertyComponent = (PropertyComponent)allComponents[index];

                        BuildProperty(propertyComponent);
                        break;
                    case ComponentType.Field:
                        // Get formatted data
                        var fieldComponent = (FieldComponent)allComponents[index];

                        BuildField(fieldComponent);
                        break;
                }
            }
        }

        // Return formatted data
        return format;
    }

    private void BuildContainer(in ContainerComponent containerComponent)
    {
        var formatContainer = FormatContainer(containerComponent);

        // Write formatted data
        Append(formatContainer);

        if (!containerComponent.IsFileScoped)
        {
            Append("{");
            IncrementIndent();
        }
        Append();
    }

    private void BuildClass(in ClassComponent classComponent)
    {
        var formatClass = FormatClass(classComponent);

        // Write formatted data
        Append(formatClass);
        Append("{");
        Append();
        IncrementIndent();
    }

    private void BuildMethod(in MethodComponent methodComponent)
    {
        var formatMethod = FormatMethod(methodComponent);

        // Write formatted data
        Append(formatMethod);
        Append("{");
        IncrementIndent();
        Append();
        DecrementIndent();
        Append("}");
        Append();
    }

    private void BuildProperty(in PropertyComponent propertyComponent)
    {

    }

    private void BuildField(in FieldComponent fieldComponent)
    {

    }
}