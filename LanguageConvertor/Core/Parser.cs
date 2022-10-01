using LanguageConvertor.Components;

namespace LanguageConvertor.Core;

internal sealed class Parser
{
    public FilePack FilePack { get; } = new FilePack();

    private Stack<IComponent> _scopeStack { get; } = new Stack<IComponent>();

    public Parser(in string[] data)
    {
        Parse(data);
    }

    private void Parse(in string[] data)
    {
        // Begin parse
        var count = data.Length;
        for (var i = 0; i < count; i++)
        {
            var line = data[i];

            // Invalid line
            if (IsEmpty(line) || IsBeginScope(line)) continue;

            // IMPORTS
            if (IsImport(line))
            {
                var import = ParseImportStatement(line);
                FilePack.AddImport(import);
            }
            // CONTAINERS
            else if (IsContainer(line))
            {
                var container = ParseContainer(line);

                FilePack.AddContainer(container);
                Feed(container, true);
            }
            // CLASSES
            else if (IsClass(line))
            {
                var @class = ParseClass(line);

                FilePack.AddClass(@class);
                Feed(@class, true);
            }
            // METHODS
            else if (IsMethod(line))
            {
                var method = ParseMethod(line);

                // Get method body if applicable
                if (!method.IsAbstract)
                {
                    var methodBody = GetMethodBody(data, ref i);
                    method.AddToBody(methodBody);
                }

                FilePack.AddMethod(method);
                Feed(method, true);
            }
            // END SCOPE
            else if (IsEndScope(line))
            {
                _scopeStack.Pop();
            }
            // FIELDS
            else if (IsField(line))
            {
                var field = ParseField(line);

                FilePack.AddField(field);
                Feed(field);
            }
            // PROPERTIES
            else if (IsProperty(line))
            {
                var property = ParseProperty(line);

                FilePack.AddProperty(property);
                Feed(property);
            }

            Console.WriteLine($"[{string.Join(':', _scopeStack)}]");
        }
    }

    #region Rules

    private static bool IsEmpty(in string line)
    {
        return string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line);
    }

    private static bool IsBeginScope(in string line)
    {
        var trimmed = line.Trim(' ', '\t');
        return trimmed.StartsWith('{');// || trimmed.StartsWith('{');
    }

    private static bool IsEndScope(in string line)
    {
        var trimmed = line.Trim(' ', '\t');
        return trimmed.StartsWith('}');// || trimmed.StartsWith('{');
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
    private static bool IsMethod(in string line)
    {
        return line.Contains('(') && !line.Contains('{');
    }
    public static bool IsField(in string line)
    {
        return !line.Contains('{');
    }

    public static bool IsProperty(in string line)
    {
        return line.Contains("{ get;") || line.Contains("set; }");
    }

    #endregion
    
    #region ParseComponents

    private static ImportComponent ParseImportStatement(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim().TrimEnd(';');

        // Get import name
        var importIndex = span.IndexOf(' ');
        var importName = span[++importIndex..].ToString();

        return new ImportComponent(importName, false);
    }

    private static ContainerComponent ParseContainer(in string line)
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

    private static ClassComponent ParseClass(in string line)
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

    private static FieldComponent ParseField(in string line)
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

    private static PropertyComponent ParseProperty(in string line)
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

    private static MethodComponent ParseMethod(in string line)
    {
        var span = line.AsSpan();
        span = span.Trim();

        var accessor = string.Empty;
        var special = string.Empty;
        var parameters = new List<ParameterComponent>();

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

        // Prep type and name
        var type = string.Empty;
        var name = string.Empty;

        // Try get constructor
        var tryConstructorIndex = span.IndexOf(' ');
        var nextIndex = (tryConstructorIndex != -1) ? tryConstructorIndex : span.IndexOf('(');
        var tryConstructor = span[..(nextIndex + 1)];
        if (tryConstructor.Contains('('))
        {
            // Constructor
            var constructorIndex = span.IndexOf('(');
            name = span[..constructorIndex].ToString();
            // type = name;

            span = span[constructorIndex..];
        }
        else
        {
            // Normal method
            // Get type
            var typeIndex = span.IndexOf(' ');
            type = span[..typeIndex++].ToString();
            span = span[typeIndex..];

            // Get name
            var nameIndex = span.IndexOf('(');
            name = span[..nameIndex++].ToString();
            span = span[nameIndex..];
        }

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
                parameters.Add(new ParameterComponent("", argType, argName));

                // Break
                if (tryArgIndex == -1) hasArgs = false;
            }
        }

        return new MethodComponent(accessor, special, type, name, parameters);
    }

    #endregion

    #region Helpers

    private static List<string> GetMethodBody(in string[] data, ref int index)
    {
        var methodBody = new List<string>();

        // Advance index passed method scope initiator
        index += 2;

        var scopeCount = 1;
        while (scopeCount > 0)
        {
            // Get line
            var line = data[index++].Trim(' ', '\t');

            // Update scope counter
            if (line.Contains('}')) --scopeCount;
            var indent = new string(' ', scopeCount * 4);
            if (line.Contains('{')) ++scopeCount;

            // Break if end all local scopes
            if (scopeCount == 0) break;

            var formattedLine = $"{indent}{line}".Trim();
            if (!string.IsNullOrEmpty(formattedLine))
            {
                methodBody.Add(formattedLine);
            }
        }

        // Rewind to method end scope to pop off scope stack
        index -= 2;
        return methodBody;
    }

    private void Feed(in IComponent component, bool isScope = false)
    {
        var currentScope = _scopeStack.TryPeek(out var scope) ? scope : null;
        currentScope?.AddComponent(component);
        FilePack.AddComponent(component);
        if (isScope)
        {
            _scopeStack.Push(component);
        }
    }

    #endregion
}