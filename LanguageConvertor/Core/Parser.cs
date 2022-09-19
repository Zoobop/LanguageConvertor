using LanguageConvertor.Components;

namespace LanguageConvertor.Core;

internal class Parser
{

    internal readonly struct ComponentPack
    {
        public IComponent Component { get; }
        public ComponentType Type { get; }

        public ComponentPack(in IComponent component, ComponentType type)
        {
            Component = component;
            Type = type;
        }
    }

    private readonly IEnumerable<string> _data = new List<string>();

    public List<string> Imports { get; } = new List<string>();
    public List<ContainerComponent> Containers { get; } = new List<ContainerComponent>();
    public List<ClassComponent> Classes { get; } = new List<ClassComponent>();
    public List<MethodComponent> Methods { get; } = new List<MethodComponent>();
    public List<FieldComponent> Fields { get; }  = new List<FieldComponent>();
    public List<PropertyComponent> Properties { get; } = new List<PropertyComponent>();
    public Queue<ComponentPack> ComponentOrder { get; } = new Queue<ComponentPack>();
    public int TotalCount { get => ComponentOrder.Count; }

    public Parser(in IEnumerable<string> data)
    {
        Parse(data);
    }

    private void Parse(in IEnumerable<string> data)
    {
        // Begin parse
        foreach (var line in data)
        {
            // Invalid line
            if (IsEmpty(line) || IsScope(line)) continue;

            // IMPORTS
            if (IsImport(line))
            {
                var import = ParseImportStatement(line);
                Imports.Add(import);
                //Console.WriteLine(import);
            }
            // CONTAINERS
            else if (IsContainer(line))
            {
                var container = ParseContainer(line);
                //Console.WriteLine(container);

                Containers.Add(container);
                ComponentOrder.Enqueue(new ComponentPack(container, ComponentType.Container));
            }
            // CLASSES
            else if (IsClass(line))
            {
                var @class = ParseClass(line);
                //Console.WriteLine(@class);

                Classes.Add(@class);
                ComponentOrder.Enqueue(new ComponentPack(@class, ComponentType.Class));
            }
            // METHODS
            else if (IsMethod(line))
            {
                var method = ParseMethod(line);
                //Console.WriteLine(method);

                Methods.Add(method);
                ComponentOrder.Enqueue(new ComponentPack(method, ComponentType.Method));
            }
            // FIELDS
            else if (IsField(line))
            {
                var field = ParseField(line);
                //Console.WriteLine(field);

                Fields.Add(field);
                ComponentOrder.Enqueue(new ComponentPack(field, ComponentType.Field));
            }
            // PROPERTIES
            else
            {
                var property = ParseProperty(line);
                //Console.WriteLine(property);

                Properties.Add(property);
                ComponentOrder.Enqueue(new ComponentPack(property, ComponentType.Property));
            }
        }
    }

    #region Rules

    private static bool IsEmpty(in string line)
    {
        return string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line);
    }

    private static bool IsScope(in string line)
    {
        var trimmed = line.Trim();
        return trimmed.StartsWith('}') || trimmed.StartsWith('{');
    }

    private static bool IsImport(in string line)
    {
        return line.StartsWith("using");
    }
    private static bool IsContainer(in string line)
    {
        return line.StartsWith("namespace");
    }
    private static bool IsClass(in string line)
    {
        return line.Contains("class");
    }
    private static bool IsMethod(string line)
    {
        return line.Contains('(') && !line.Contains('{');
    }
    public static bool IsField(string line)
    {
        return !line.Contains('{');
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
        var hasSpecial = span.StartsWith("static") || span.StartsWith("sealed") || span.StartsWith("abstract");
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
}