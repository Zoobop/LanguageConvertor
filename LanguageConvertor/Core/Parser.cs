using LanguageConvertor.Languages;
using LanguageConvertor.Components;
using LanguageConvertor.Modifiers;
using System.Reflection;

namespace LanguageConvertor.Core;

internal class Parser
{
    private enum ScopeType
    {
        Container,
        Class,
        Property,
        Method
    }

    private struct Scope
    {
        public string Name { get; set; } = string.Empty;
        public Stack<ScopeType> Type { get; set; } = new Stack<ScopeType>();
    }

    private readonly IEnumerable<string> _data = new List<string>();

    private List<string> _imports = new List<string>();
    private List<ContainerComponent> _containers = new List<ContainerComponent>();
    private List<ClassComponent> _classes = new List<ClassComponent>();
    private List<MethodComponent> _methods = new List<MethodComponent>();
    private List<FieldComponent> _fields = new List<FieldComponent>();
    private List<PropertyComponent> _properties = new List<PropertyComponent>();

    private readonly Scope _currentScope = new Scope();

    public Parser(in IEnumerable<string> data)
    {
        // Begin parse
        foreach (var line in data)
        {
            // Invalid line
            if (IsEmpty(line)) continue;

            // IMPORTS
            if (IsImport(line))
            {
                var importName = ParseImportStatement(line);
                _imports.Add(importName);
                Console.WriteLine(importName);
            }
            // CONTAINERS
            else if (IsContainer(line))
            {
                var container = ParseContainer(line);
                _containers.Add(container);
                Console.WriteLine(container);
            }
            // CLASSES
            else if (IsClass(line))
            {
                var @class = ParseClass(line);
                _classes.Add(@class);
                Console.WriteLine(@class);
            }
            // METHODS
            else if (IsMethod(line))
            {
                var method = ParseMethod(line);
                _methods.Add(method);
                Console.WriteLine(method);
            }
            // FIELDS
            else if (IsField(line))
            {
                var field = ParseField(line);
                _fields.Add(field);
                Console.WriteLine(field);
            }
        }
        
    }
    
    #region Getters

    public List<string> GetImports()
    {
        return _imports;
    }

    public List<ContainerComponent> GetContainers()
    {
        return _containers;
    }
    
    public List<ClassComponent> GetClasses()
    {
        return _classes;
    }
    
    public List<MethodComponent> GetMethods()
    {
        return _methods;
    }

    public List<PropertyComponent> GetProperties()
    {
        return _properties;
    }

    public List<FieldComponent> GetFields()
    {
        return _fields;
    }

    #endregion

    #region Rules

    private static bool IsEmpty(in string line)
    {
        return string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line);
    }

    public static bool IsImport(in string line)
    {
        return line.StartsWith("using");
    }
    public static bool IsContainer(in string line)
    {
        return line.StartsWith("namespace");
    }
    public static bool IsClass(in string line)
    {
        return line.Contains("class");
    }
    public static bool IsMethod(string line)
    {
        return line.Contains('(') && !line.Contains('{');
    }
    public static bool IsField(string line)
    {
        return false;

        var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (split.Length <= 0) return false;
        var hasAccess = CheckAccessModifier(split.First());
        return !IsContainer(line) && !IsClass(line) && !IsMethod(line) && hasAccess;
    }
    public static bool IsProperty(string line)
    {
        if (!(line.Contains('{') && line.Contains('}'))) return false;

        var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var hasAccess = CheckAccessModifier(split.First());

        return !IsField(line) && hasAccess;
    }
    public static bool IsScope(string line, bool begin)
    {
        var text = line.TrimStart(' ');
        return begin ? text.StartsWith('{') : text.StartsWith('}');
    }

    #endregion
    
    #region ParseComponents

    private string ParseImportStatement(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim().TrimEnd(';');

        // Get import name
        var importIndex = span.IndexOf(' ');
        var importName = span[++importIndex..].ToString();

        return importName;
    }

    private ContainerComponent ParseContainer(in string line)
    {
        var span = line.AsSpan();
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

    private ClassComponent ParseClass(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var name = string.Empty;
        var parent = string.Empty;
        var interfaces = new List<string>();

        // Try get accessor
        var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasAccess)
        {
            var length = span.IndexOf(' ');
            accessor = span[..length++].ToString();
            //Console.WriteLine($"[{accessor}]");
            span = span[length..];
        }
        
        // Try get special
        var hasSpecial = span.StartsWith("static") || span.StartsWith("sealed") || span.StartsWith("virtual") || span.StartsWith("abstract");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }

        // Skip 'class' keyword
        var classIndex = span.IndexOf(' ');
        span = span[++classIndex..];

        // Get name
        var tryNameIndex = span.IndexOf(' ');
        if (tryNameIndex == -1)
        {
            name = span.ToString();
        }
        else
        {
            name = span[..tryNameIndex++].ToString();
            //Console.WriteLine($"[{name}]");
            span = span[tryNameIndex..];
        }

        // Try get parents
        var baseIndex = span.IndexOf(':');
        if (baseIndex != -1)
        {
            var startIndex = baseIndex + 2;
            span = span[startIndex..].Trim();

            var hasParents = true;
            while (hasParents)
            {
                // Get parent
                var tryParentIndex = span.IndexOf(',');
                var currentParent = string.Empty;
                if (tryParentIndex == -1)
                {
                    // Last one
                    currentParent = span.ToString();
                }
                else
                {
                    currentParent = span[..tryParentIndex++].ToString();
                    span = span[++tryParentIndex..];
                }

                // Interface check
                if (currentParent.StartsWith('I'))
                {
                    // Interface
                    interfaces.Add(currentParent);
                }
                else
                {
                    // Class
                    parent = currentParent;
                }

                // Break
                if (tryParentIndex == -1) hasParents = false;
            }
        }

        return new ClassComponent(accessor, special, name, parent, interfaces);
    }

    private FieldComponent ParseField(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var value = string.Empty;

        // Try get accessor
        var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasAccess)
        {
            var length = span.IndexOf(' ');
            accessor = span[..length++].ToString();
            //Console.WriteLine($"[{accessor}]");
            span = span[length..];
        }

        // Try get special
        var hasSpecial = span.StartsWith("static") || span.StartsWith("const");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }

        // Get type
        var typeIndex = span.IndexOf(' ');
        var type = span[..typeIndex++].ToString();
        //Console.WriteLine($"[{type}]");
        span = span[typeIndex..];

        // Get name
        var tryIndex = span.IndexOf(' ');
        var nameIndex = (tryIndex == -1) ? span.IndexOf(';') : tryIndex;
        var name = span[..nameIndex++].ToString();
        //Console.WriteLine($"[{name}]");
        span = span[nameIndex..];

        // Try get value
        var valueIndex = span.IndexOf('=');
        if (valueIndex != -1)
        {
            var index = valueIndex + 2; ;
            value = span[index..^1].TrimEnd(';').ToString();
            //Console.WriteLine($"[{value}]");
        }

        return new FieldComponent(accessor, special, type, name, value);
    }

    private PropertyComponent ParseProperty(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var canRead = false;
        var canWrite = false;
        var writeAccessModifier = string.Empty;
        var value = string.Empty;

        // Try get accessor
        var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasAccess)
        {
            var length = span.IndexOf(' ');
            accessor = span[..length++].ToString();
            //Console.WriteLine($"[{accessor}]");
            span = span[length..];
        }

        // Try get special
        var hasSpecial = span.StartsWith("static") || span.StartsWith("const");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }

        // Get type
        var typeIndex = span.IndexOf(' ');
        var type = span[..typeIndex++].ToString();
        //Console.WriteLine($"[{type}]");
        span = span[typeIndex..];

        // Get name
        var tryIndex = span.IndexOf(' ');
        var nameIndex = (tryIndex == -1) ? span.IndexOf(';') : tryIndex;
        var name = span[..nameIndex++].ToString();
        //Console.WriteLine($"[{name}]");
        span = span[nameIndex..];

        // Try get getter
        var getterIndex = span.IndexOf('g');
        if (getterIndex != -1)
        {
            canRead = true;
            var nextIndex = getterIndex + 5;
            span = span[nextIndex..];
            //Console.WriteLine($"[{canRead}]");
        }

        // Try get write accessor
        var hasWriteAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasWriteAccess)
        {
            var length = span.IndexOf(' ');
            writeAccessModifier = span[..length++].ToString();
            //Console.WriteLine($"[{writeAccessModifier}]");
            span = span[length..];
        }

        // Try get setter
        var setterIndex = span.IndexOf('s');
        if (setterIndex != -1)
        {
            canWrite = true;
            //Console.WriteLine($"[{canWrite}]");
        }

        // Try get value
        var valueIndex = span.IndexOf('=');
        if (valueIndex != -1)
        {
            var index = valueIndex + 2; ;
            value = span[index..^1].TrimEnd(';').ToString();
            //Console.WriteLine($"[{value}]");
        }

        return new PropertyComponent(accessor, special, type, name, value, canRead, canWrite, writeAccessModifier);
    }

    private MethodComponent ParseMethod(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var parameters = new Dictionary<string, string>();

        // Try get accessor
        var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
        if (hasAccess)
        {
            var length = span.IndexOf(' ');
            accessor = span[..length++].ToString();
            //Console.WriteLine($"[{accessor}]");
            span = span[length..];
        }
        
        // Try get special
        var hasSpecial = span.StartsWith("static") || span.StartsWith("override") || span.StartsWith("virtual");
        if (hasSpecial)
        {
            var length = span.IndexOf(' ');
            special = span[..length++].ToString();
            //Console.WriteLine($"[{special}]");
            span = span[length..];
        }
        
        // Get type
        var typeIndex = span.IndexOf(' ');
        var type = span[..typeIndex++].ToString();
        //Console.WriteLine($"[{type}]");
        span = span[typeIndex..];

        // Get name
        var nameIndex = span.IndexOf('(');
        var name = span[..nameIndex].ToString();
        //Console.WriteLine($"[{name}]");
        span = span[nameIndex..];

        // Try get params
        var startParenthIndex = span.IndexOf('(');
        var endParenthIndex = span.IndexOf(')');
        if (endParenthIndex - startParenthIndex > 1)
        {
            span = span.TrimStart('(');
            var hasArgs = true;
            while (hasArgs)
            {
                // Get arg type
                var argTypeIndex = span.IndexOf(' ');
                var argType = span[..argTypeIndex++].ToString();
                span = span[argTypeIndex..];

                // Get arg name
                var tryArgIndex = span.IndexOf(',');
                var argNameIndex = (tryArgIndex == -1) ? span.IndexOf(')') : tryArgIndex;
                var argName = span[..argNameIndex++].ToString();
                span = span[argNameIndex..].Trim();

                //Console.WriteLine($"[{argType}:{argName}]");
                parameters.Add(argName, argType);

                // Break
                if (tryArgIndex == -1) hasArgs = false;
            }
        }

        return new MethodComponent(accessor, special, type, name, parameters);
    }

    #endregion

    private static bool CheckAccessModifier(string text)
    {
        return text is "private" or "protected" or "public";
    }

    private static bool CheckSpecialModifier(string text)
    {
        return text is "abstract" or "override" or "virtual";
    }

    private static bool CheckClassSpecialModifier(string text)
    {
        return text is "abstract" or "sealed";
    }
}