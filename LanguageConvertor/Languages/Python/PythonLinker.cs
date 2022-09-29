using LanguageConvertor.Components;
using LanguageConvertor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Languages;

internal sealed class PythonLinker : Linker
{
    private static readonly IDictionary<string, string> _commonTypeConversions = new Dictionary<string, string>
    {
        {"void", "None"},
        {"bool", "bool"},
        {"byte", "int"},
        {"char", "char"},
        {"short", "str"},
        {"int", "int"},
        {"long", "int"},
        {"object", "object"},
        {"string", "str"},
    };

    private static readonly string _arrayConversion = "list[{0}]";

    public PythonLinker(string[] data) : base(data)
    {
    }

    public PythonLinker(in FilePack filePack) : base(filePack)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Python;
    }

    protected override string GetImportKeyword()
    {
        return "import";
    }

    protected override string GetContainerKeyword()
    {
        return string.Empty;
    }

    #region Formatting

    protected override string FormatImport(string importName)
    {
        return $"from {importName} {GetImportKeyword()} *";
    }

    protected override string FormatContainer(in ContainerComponent containerComponent)
    {
        // Format container
        return string.Empty;
    }

    protected override string FormatClass(in ClassComponent classComponent)
    {
        // Format class definition
        var format = new StringBuilder();

        // Add '@dataclass' attribute
        format.Append($"@dataclass\n{GetCurrentIndent()}");

        // Add 'class' keyword
        format.Append("class ");

        // Add name
        var name = classComponent.Name;
        format.Append(name);

        // Try add inherited classes
        var parentClass = classComponent.ParentClass;
        var interfaces = classComponent.Interfaces;
        var inheritedClasses = new List<string>();

        // Parent class
        if (!string.IsNullOrEmpty(parentClass))
        {
            inheritedClasses.Add(parentClass);
        }

        // Interfaces
        if (interfaces.Count > 0)
        {
            inheritedClasses.AddRange(interfaces);
        }

        if (inheritedClasses.Count > 0)
        {
            format.Append($"({string.Join(", ", inheritedClasses)})");
        }

        // Add ':'
        format.Append(':');

        return format.ToString();
    }

    protected override string FormatMethod(in MethodComponent methodComponent)
    {
        // Format method definition
        var format = new StringBuilder();

        // Add 'def' keyword
        format.Append("def ");

        // Add name
        var name = methodComponent.Name;
        format.Append(ConvertMethodNameToPython(name));

        // Try add parameters
        var hasParameters = methodComponent.HasParameters;
        format.Append("(self");
        if (hasParameters)
        {
            // Format each parameter
            format.Append(", ");
            foreach (var (argName, argType) in methodComponent.Parameters)
            {
                format.Append($"{argName}: {TryConvertTypeToPython(argType)}, ");
            }

            // Trim end
            format.Remove(format.Length - 2, 2);
        }
        format.Append(')');

        // Add return type
        var returnType = methodComponent.Type;
        if (methodComponent.IsConstructor)
        {
            format.Append(" -> None:");
        }
        else
        {
            
            format.Append($" -> {TryConvertTypeToPython(returnType)}:");
        }


        // Try get method body
        if (methodComponent.Body.Count == 0)
        {
            methodComponent.AddToBody("pass");
        }

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
        var type = TryConvertTypeToPython(propertyComponent.Type);
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

        format.Append('}');

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

        // Add name
        var name = fieldComponent.Name;
        format.Append(name);

        // Try add value
        var value = fieldComponent.Value;
        if (!string.IsNullOrEmpty(value))
        {
            format.Append($" = {value}");
        }
        else
        {
            // Add type
            var type = TryConvertTypeToPython(fieldComponent.Type);
            format.Append($": {type}");
        }

        return format.ToString();
    }

    #endregion

    #region Construction

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

            DecrementIndent();
            Append();
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
            // Remove account property
            _parser.Components.Remove(property);

            // Create the backing field
            var span = property.Name.AsSpan();
            var name = $"{span[0].ToString().ToLower()}{span[1..]}";
            var fieldComponent = new FieldComponent("private", property.SpecialModifier, property.Type, $"{name}BackingField", property.Value);
            classComponent.AddField(fieldComponent);

            // Try create getter
            if (property.CanRead)
            {
                var methodComponent = new MethodComponent(property.AccessModifier, property.SpecialModifier, property.Type, $"get{property.Name}");
                methodComponent.AddToBody($"return {fieldComponent.Name}");
                classComponent.AddMethod(methodComponent);
            }

            // Try create setter
            if (property.CanWrite)
            {
                var accessor = (!string.IsNullOrEmpty(property.WriteAccessModifier)) ? property.WriteAccessModifier : "public";

                var argName = "value";
                var methodComponent = new MethodComponent(accessor, property.SpecialModifier, "void", $"set{property.Name}", new KeyValuePair<string, string>(argName, TryConvertTypeToPython(property.Type)));
                methodComponent.AddToBody($"self.{fieldComponent.Name} = {argName}");
                classComponent.AddMethod(methodComponent);
            }
        }
    }

    #endregion

    #region Building

    public override IEnumerable<string> BuildFileLines()
    {
        // FORMATTING

        // Build containers
        var containers = _parser.Containers;
        foreach (var container in containers)
        {
            // Remove accounted container
            _parser.Components.Remove(container);
            BuildContainer(container);

            // Imports
            foreach (var import in _parser.Imports)
            {
                Append(FormatImport(import));
            }
            Append();

            // Build classes
            ConstructClass(container.Classes);

            // Close scopes
            while (_indentLevel != 0)
            {
                DecrementIndent();
                Append("}");
            }
        }

        // Return formatted data
        return _formattedData;
    }

    public override string BuildFile()
    {
        var lines = BuildFileLines();
        var builder = new StringBuilder();
        foreach (var line in lines)
        {
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    private void BuildContainer(in ContainerComponent containerComponent)
    {
        var formatContainer = FormatContainer(containerComponent);

        // Write formatted data
        Append(formatContainer);
        Append();
    }

    private void BuildClass(in ClassComponent classComponent)
    {
        var formatClass = FormatClass(classComponent);

        // Write formatted data
        Append(formatClass);
        IncrementIndent();
    }

    private void BuildMethod(in MethodComponent methodComponent)
    {
        var formatMethod = FormatMethod(methodComponent);

        // Write formatted data
        Append(formatMethod);
        IncrementIndent(); // Enter scope

        // Method body
        if (methodComponent.Body.Count > 0)
        {
            foreach (var line in methodComponent.Body)
            {
                Append(line);
            }
        }
        else
        {
            Append("pass");
        }


        DecrementIndent(); // Exit scope
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

    #endregion

    #region Helpers

    private static string TryConvertTypeToPython(string type)
    {
        // Convert array types
        var span = type.AsSpan();
        var typeName = type;
        var format = false;
        var tryArrayIndex = span.IndexOf('[');
        if (tryArrayIndex != -1)
        {
            typeName = span[..tryArrayIndex].ToString();
            format = true;
        }

        // Convert type name
        if (_commonTypeConversions.TryGetValue(typeName, out var pythonType))
        {
            return (!format) ? pythonType : string.Format(_arrayConversion, pythonType);
        }

        return (!format) ? type : string.Format(_arrayConversion, type);
    }

    private static string ConvertMethodNameToPython(string name)
    {
        var span = name.AsSpan();
        var pythonName = span[0..1];

        return $"{pythonName.ToString().ToLower()}{span[1..].ToString()}";
    }

    #endregion
}
