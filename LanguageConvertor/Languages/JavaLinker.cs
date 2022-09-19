using LanguageConvertor.Components;
using LanguageConvertor.Core;
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
        var format = (containerComponent.IsFileScoped) ? $"{containerComponent.Name};" : containerComponent.Name;
        return $"{GetContainerKeyword()} {format}";
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

        // Try add name
        var name = classComponent.Name;
        if (!string.IsNullOrEmpty(name))
        {
            format.Append(name);
        }

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
        throw new NotImplementedException();
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
                        var formatContainer = FormatContainer(containerComponent);

                        // Write formatted data
                        Append(formatContainer);

                        if (!containerComponent.IsFileScoped)
                        {
                            Append("{");
                            IncrementIndent();
                        }
                        Append();
                        
                        break;
                    case ComponentType.Class:
                        // Get formatted data
                        var classComponent = (ClassComponent)allComponents[index];
                        var formatClass = FormatClass(classComponent);

                        // Write formatted data
                        Append(formatClass);
                        Append("{");
                        Append();
                        IncrementIndent();
                        break;
                    case ComponentType.Method:
                        var methodComponent = (MethodComponent)allComponents[index];

                        //var formatMethod = FormatMethod(methodComponent);
                        //Append(formatMethod);
                        break;
                    case ComponentType.Property:
                        break;
                    case ComponentType.Field:
                        break;
                }
            }
        }

        // Return formatted data
        return format;
    }
}