using LanguageConvertor.Components;
using LanguageConvertor.Core;
using System;
using System.Text;
using System.Collections.Generic;

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
        // Format property definition
        var format = new StringBuilder();

        // Try add accessor
        var accessor = propertyComponent.AccessModifier;
        if (!string.IsNullOrEmpty(accessor))
        {
            format.Append($"{accessor} ");
        }

        // Try add special
        var special = propertyComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special))
        {
            format.Append($"{special} ");
        }

        // Add type
        var type = propertyComponent.Type;
        format.Append($"{type} ");

        // Add name
        var name = propertyComponent.Name;
        format.Append(name);

        // Try add getter
        var canRead = propertyComponent.CanRead;
        format.Append(" { ");
        if (canRead)
        {
            format.Append("get; ");
        }

        // Try get write accessor
        var writeAccessor = propertyComponent.WriteAccessModifier;
        if (!string.IsNullOrEmpty(writeAccessor))
        {
            format.Append($"{writeAccessor} ");
        }

        // Try get setter
        var canWrite = propertyComponent.CanWrite;
        if (canWrite)
        {
            format.Append("set; ");
        }

        format.Append("}");

        // Try get value
        var value = propertyComponent.Value;
        if (!string.IsNullOrEmpty(value))
        {
            format.Append($" = {value};");
        }

        return format.ToString();
    }

    protected override string FormatField(in FieldComponent fieldComponent)
    {
        // Format field definition
        var format = new StringBuilder();

        // Try add accessor
        var accessor = fieldComponent.AccessModifier;
        if (!string.IsNullOrEmpty(accessor))
        {
            format.Append($"{accessor} ");
        }

        // Try add special
        var special = fieldComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special))
        {
            format.Append($"{special} ");
        }

        // Add type
        var type = fieldComponent.Type;
        format.Append($"{type} ");

        // Add name
        var name = fieldComponent.Name;
        format.Append(name);

        // Try add value
        var value = fieldComponent.Value;
        if (!string.IsNullOrEmpty(value))
        {
            format.Append($" = {value}");
        }

        // Add end-statement indicator
        format.Append(';');

        return format.ToString();
    }

    protected override void ConstructClass(List<ClassComponent> classes)
    {
        foreach (var classComponent in classes)
        {
            // Remove accounted class
            _parser.Components.Remove(classComponent);
            BuildClass(classComponent);

            // Convert properties
            ConvertProperty(classComponent);

            // Build fields
            ConstructFields(classComponent.Fields);

            // Build methods
            ConstructMethods(classComponent.Methods);
        }
    }

    protected override void ConstructMethods(List<MethodComponent> methods)
    {
        var publicMethods = methods.Where(x => x.AccessModifier == "public");
        var privateMethods = methods.Where(x => x.AccessModifier == "private" || string.IsNullOrEmpty(x.AccessModifier));
        var protectedMethods = methods.Where(x => x.AccessModifier == "protected");

        // Build public methods
        foreach (var pub in publicMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(pub);
            BuildMethod(pub);
        }

        // Build private methods
        foreach (var priv in privateMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(priv);
            BuildMethod(priv);
        }

        // Build protected methods
        foreach (var prot in protectedMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(prot);
            BuildMethod(prot);
        }
    }

    protected override void ConstructFields(List<FieldComponent> fields)
    {
        foreach (var field in fields)
        {
            // Remove accounted field
            _parser.Components.Remove(field);
            BuildField(field);
        }

        Append();
    }

    protected override void ConvertProperty(in ClassComponent classComponent)
    {
        foreach (var property in classComponent.Properties)
        {
            // Create the backing field
            var span = property.Name.AsSpan();
            var name = $"{span[0].ToString().ToLower()}{span[1..]}";
            var fieldComponent = new FieldComponent("private", property.SpecialModifier, property.Type, $"{name}BackingField", property.Value);
            classComponent.AddField(fieldComponent);

            // Try create getter
            if (property.CanRead)
            {
                var methodComponent = new MethodComponent(property.AccessModifier, property.SpecialModifier, property.Type, $"get{property.Name}");
                classComponent.AddMethod(methodComponent);
            }

            // Try create setter
            if (property.CanWrite)
            {
                var accessor = (!string.IsNullOrEmpty(property.WriteAccessModifier)) ? property.WriteAccessModifier : "public";

                var methodComponent = new MethodComponent(accessor, property.SpecialModifier, "void", $"set{property.Name}", new KeyValuePair<string, string>("value", property.Type));
                classComponent.AddMethod(methodComponent);
            }
        }
    }

    public override IEnumerable<string> BuildFile()
    {
        // FORMATTING
        var format = new List<string>(_parser.TotalCount);

        // Imports
        foreach (var import in _parser.Imports)
        {
            Append(FormatImport(import));
        }
        Append();

        // Build containers
        var containers = _parser.Containers;
        foreach (var container in containers)
        {
            // Remove accounted container
            _parser.Components.Remove(container);
            BuildContainer(container);

            // Build classes
            ConstructClass(container.Classes);

            // Close scopes
            while (_indentLevel != 0)
            {
                DecrementIndent();
                Append("}");
            }
        }

        // Build classes
        if (_parser.Components.Count > 0)
        {
            Append();

            var classes = _parser.Components
                .Where(x => x.GetType() == typeof(ClassComponent))
                .Select(x => (ClassComponent)x)
                .ToList();

            // Build classes
            ConstructClass(classes);

            // Close scopes
            while (_indentLevel != 0)
            {
                DecrementIndent();
                Append("}");
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
        IncrementIndent();
    }

    private void BuildMethod(in MethodComponent methodComponent)
    {
        var formatMethod = FormatMethod(methodComponent);

        // Write formatted data
        Append(formatMethod);
        Append("{"); // Enter scope
        IncrementIndent();
        
        Append(); // Method body

        DecrementIndent();
        Append("}"); // Exit scope
        Append();
    }

    private void BuildProperty(in PropertyComponent propertyComponent)
    {
        var formatProperty = FormatProperty(propertyComponent);

        // Write formatted data
        Append(formatProperty);
        Append();
    }

    private void BuildField(in FieldComponent fieldComponent)
    {
        var formatField = FormatField(fieldComponent);

        // Write formatted data
        Append(formatField);
    }
}