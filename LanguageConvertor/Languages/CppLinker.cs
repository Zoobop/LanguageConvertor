using LanguageConvertor.Components;
using LanguageConvertor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Languages;

internal sealed class CppLinker : Linker
{
    private static readonly IDictionary<string, string> _commonTypeConversions = new Dictionary<string, string>
    {
        {"bool", "bool"},
        {"sbyte", "int8_t"},
        {"char", "int8_t"},
        {"short", "int16_t"},
        {"int", "int32_t"},
        {"long", "int64_t"},
        {"byte", "uint8_t"},
        {"ushort", "uint16_t"},
        {"uint", "uint32_t"},
        {"ulong", "uint64_t"},
        {"object", "uint32_t"},
        {"string", "std::string"},
    };

    public CppLinker(string[] data) : base(data)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Cpp;
    }

    protected override string GetImportKeyword()
    {
        return "#include";
    }

    protected override string GetContainerKeyword()
    {
        return "namespace";
    }

    #region Formatting

    protected override string FormatImport(string importName)
    {
        return $"{GetImportKeyword()} \"Example/{importName}.hpp\"";
    }

    protected override string FormatContainer(in ContainerComponent containerComponent)
    {
        // Format container
        return $"{GetContainerKeyword()} {containerComponent.Name}";
    }

    protected override string FormatClass(in ClassComponent classComponent)
    {
        // Format class definition
        var format = new StringBuilder();

        // Add 'class' keyword
        format.Append("class ");

        // Add name
        var name = classComponent.Name;
        format.Append(name);

        // Try add parent class
        var parentClass = classComponent.ParentClass;
        if (!string.IsNullOrEmpty(parentClass))
        {
            format.Append($" : public {parentClass}");
        }

        // Try add interface(s)
        var interfaces = classComponent.Interfaces;
        if (interfaces.Count > 0)
        {
            format.Append($", public {string.Join(", public ", interfaces)}");
        }

        return format.ToString();
    }

    protected override string FormatMethod(in MethodComponent methodComponent)
    {
        // Format method definition
        var format = new StringBuilder();

        // Try add accessor
        //var accessor = methodComponent.AccessModifier;
        //if (!string.IsNullOrEmpty(accessor))
        //{
        //    format.Append($"{accessor} ");
        //}

        // Try add special
        var special = methodComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special) && !methodComponent.IsOverride)
        {
            format.Append($"{special} ");
        }

        // Add return type
        var returnType = TryConvertTypeToCpp(methodComponent.Type);
        format.Append($"{returnType} ");

        // Add name
        var name = methodComponent.Name;
        format.Append(ConvertMethodNameToCpp(name));

        // Try add parameters
        var hasParameters = methodComponent.HasParameters;
        format.Append('(');
        if (hasParameters)
        {
            // Format each parameter
            foreach (var (argName, argType) in methodComponent.Parameters)
            {
                format.Append($"{TryConvertTypeToCpp(argType)} {argName}, ");
            }

            // Trim end
            format.Remove(format.Length - 2, 2);
        }
        format.Append(')');

        // Try add override
        if (!string.IsNullOrEmpty(special) && methodComponent.IsOverride)
        {
            format.Append($" {special}");
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
        var type = TryConvertTypeToCpp(propertyComponent.Type);
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

        // Try add special
        var special = fieldComponent.SpecialModifier;
        if (!string.IsNullOrEmpty(special))
        {
            format.Append($"{special} ");
        }

        // Add type
        var type = TryConvertTypeToCpp(fieldComponent.Type);
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

            // Build methods
            ConstructMethods(classComponent.Methods);

            // Build fields
            ConstructFields(classComponent.Fields);

            DecrementIndent();
            Append("}");
            Append();
        }
    }

    protected override void ConstructMethods(List<MethodComponent> methods)
    {
        var publicMethods = methods.Where(x => x.AccessModifier == "public");
        var privateMethods = methods.Where(x => x.AccessModifier == "private" || string.IsNullOrEmpty(x.AccessModifier));
        var protectedMethods = methods.Where(x => x.AccessModifier == "protected");

        // Build public methods
        Append("public:");
        IncrementIndent();
        foreach (var pub in publicMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(pub);
            BuildMethod(pub);
        }

        // Build protected methods
        DecrementIndent();
        Append("protected:");
        IncrementIndent();
        foreach (var prot in protectedMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(prot);
            BuildMethod(prot);
        }
    
        // Build private methods
        DecrementIndent();
        Append("private:");
        IncrementIndent();
        foreach (var priv in privateMethods)
        {
            // Remove accounted method
            _parser.Components.Remove(priv);
            BuildMethod(priv);
        }

        DecrementIndent();
    }

    protected override void ConstructFields(List<FieldComponent> fields)
    {
        var publicFields = fields.Where(x => x.AccessModifier == "public").ToList();
        var privateFields = fields.Where(x => x.AccessModifier == "private" || string.IsNullOrEmpty(x.AccessModifier)).ToList();
        var protectedFields = fields.Where(x => x.AccessModifier == "protected").ToList();

        // Try build public fields
        if (publicFields != null && publicFields.Count > 0)
        {
            Append("public:");
            IncrementIndent();
            foreach (var pub in publicFields)
            {
                // Remove accounted field
                _parser.Components.Remove(pub);
                BuildField(pub);
            }
            Append();
            DecrementIndent();
        }

        // Try build protected fields
        if (protectedFields != null && protectedFields.Count > 0)
        {
            Append("protected:");
            IncrementIndent();
            foreach (var prot in protectedFields)
            {
                // Remove accounted field
                _parser.Components.Remove(prot);
                BuildField(prot);
            }
            Append();
            DecrementIndent();
        }
    
        // Try build private fields
        Append("private:");
        IncrementIndent();
        foreach (var priv in privateFields)
        {
            // Remove accounted field
            _parser.Components.Remove(priv);
            BuildField(priv);
        }
        Append();
        DecrementIndent();
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

            var currentIndent = new string(' ', _indentLevel * 4);

            // Try create getter
            if (property.CanRead)
            {
                var methodComponent = new MethodComponent(property.AccessModifier, property.SpecialModifier, property.Type, $"get{property.Name}");
                methodComponent.AddToBody($"{currentIndent}return {fieldComponent.Name};");
                classComponent.AddMethod(methodComponent);
            }

            // Try create setter
            if (property.CanWrite)
            {
                var accessor = (!string.IsNullOrEmpty(property.WriteAccessModifier)) ? property.WriteAccessModifier : "public";

                var argName = "value";
                var methodComponent = new MethodComponent(accessor, property.SpecialModifier, "void", $"set{property.Name}", new KeyValuePair<string, string>(argName, TryConvertTypeToCpp(property.Type)));
                methodComponent.AddToBody($"{currentIndent}{fieldComponent.Name} = {argName};");
                classComponent.AddMethod(methodComponent);
            }
        }
    }

    #endregion

    #region FileBuilding

    public override IEnumerable<string> BuildFileLines()
    {
        // FORMATTING

        // Pragma once
        Append("#pragma once");
        Append();

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
        Append("{");
        IncrementIndent();
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

        // Method body
        foreach (var line in methodComponent.Body)
        {
            Append(line);
        }

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

    #endregion

    #region Helpers

    private static string TryConvertTypeToCpp(string type)
    {
        // TO-DO: Convert array types
        var span = type.AsSpan();
        var typeName = type;
        var suffix = string.Empty;
        var tryArrayIndex = type.Index('[');
        if (tryArrayIndex != -1)
        {
            span = span[..tryArrayIndex];
        }

        // Convert type name
        if (_commonTypeConversions.TryGetValue(type, out var javaType))
        {
            return $"{type}";
        }

        return $"{type}";
    }

    private static string ConvertMethodNameToCpp(string name)
    {
        var span = name.AsSpan();
        var cppName = span[0..1];

        return $"{cppName.ToString().ToLower()}{span[1..].ToString()}";
    }

    #endregion
}
