using LanguageConvertor.Languages;
using LanguageConvertor.Modifiers;
using System.Reflection;

namespace LanguageConvertor.Core;

public class Parser
{
    private readonly IEnumerable<string> _data;

    public Parser(IEnumerable<string> data)
    {
        _data = data;
    }
    
    public List<string> GetImports()
    {
        return (from line in _data where IsImport(line) select line[6..^1]).ToList();
    }

    public ContainerSettings GetContainer(ConvertibleLanguage language)
    {
        var fileScoped = language switch
        {
            ConvertibleLanguage.Cpp or 
                ConvertibleLanguage.Python => false,
            _ => true
        };
        
        var containerName = "";
        foreach (var line in _data)
        {
            if (!IsContainer(line)) continue;
            containerName = line.Remove(0, 10).TrimEnd(';');
            break;
        }
        return new(containerName, fileScoped);
    }
    
    public Dictionary<string, ClassModifiers> GetClassModifiers()
    {
        var classes = new Dictionary<string, ClassModifiers>();
        foreach (var line in _data)
        {
            if (!IsClass(line)) continue;
            var (name, modifiers) = ParseClassModifiers(line);
            classes.Add(name, modifiers);
        }

        return classes;
    }
    
    public Dictionary<string, MethodModifiers> GetMethodModifiers()
    {
        var methods = new Dictionary<string, MethodModifiers>();
        for (var i = 0; i < _data.Count(); i++)
        {
            var line = _data.ElementAt(i);
            if (!IsMethod(line)) continue;
            var (name, modifiers) = ParseMethodModifiers(line, i);
            methods.Add(name, modifiers);
        }
        
        return methods;
    }
    
    public Dictionary<string, MemberModifiers> GetMemberModifiers()
    {
        var members = new Dictionary<string, MemberModifiers>();
        foreach (var line in _data)
        {
            if (!IsMember(line)) continue;
            var (name, modifiers) = ParseMemberModifiers(line);
            members.Add(name, modifiers);
        }
        
        return members;
    }

    public static bool IsImport(string line)
    {
        return line.StartsWith("using");
    }
    public static bool IsContainer(string line)
    {
        return line.StartsWith("namespace");
    }
    public static bool IsClass(string line)
    {
        return line.Contains("class");
    }
    public static bool IsMethod(string line)
    {
        return line.Contains('(');
    }
    public static bool IsMember(string line)
    {
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

        return !IsMember(line) && hasAccess;
    }
    public static bool IsScope(string line, bool begin)
    {
        var text = line.TrimStart(' ');
        return begin ? text.StartsWith('{') : text.StartsWith('}');
    }
    
    private (string, ClassModifiers) ParseClassModifiers(string line)
    {
        var trimmedLine = line.TrimStart(' ');
        var index = trimmedLine.IndexOf(':');
        if (index > 0)
        {
            index = trimmedLine[..index].Count(x => x is ' ') + 2;
        }
        
        var split = index > 0 ? trimmedLine.Split(' ', index) : trimmedLine.Split(' ');
        
        var access = string.Empty;
        var special = string.Empty;
        var inheritance = new List<string>();
        var name = string.Empty;

        switch (split.Length)
        {
            // (accessor/special) class (name)
            case 3:
            {
                if (CheckAccessModifier(split.First()))
                {
                    access = split[0];
                }
                else if (CheckClassSpecialModifier(split.First()))
                {
                    special = split[0];
                }

                name = split.Last();
                break;
            }
            // (accessor) (special) class (name) or class (name) : (inheritance)
            case 4:
            {
                if (line.Contains(':'))
                {
                    name = split[1];
                    inheritance.AddRange(split[3].Replace(" ", "").Split(','));
                    break;
                }

                access = split[0];
                special = split[1];
                name = split.Last();
                break;
            }
            // (accessor) (special) class (name) : (inheritance)
            case 6:
                access = split[0];
                special = split[1];
                name = split[3];
                inheritance.AddRange(split[5].Replace(" ", "").Split(','));
                break;
            // (accessor/special) class (name) : (inheritance)
            case 5:
            {
                if (CheckAccessModifier(split.First()))
                {
                    access = split[0];
                }
                else if (CheckClassSpecialModifier(split.First()))
                {
                    special = split[0];
                }
            
                name = split[2];
                inheritance.AddRange(split[4].Replace(" ", "").Split(','));
                break;
            }
        }

        return (name, new(access, special, inheritance));
    }
    private (string, MethodModifiers) ParseMethodModifiers(string line, int startIndex)
    {
        var trimmedLine = line.TrimStart(' ');
        var index = trimmedLine.IndexOf('(');
        if (index > 0)
        {
            index = trimmedLine[..index].Count(x => x is ' ') + 1;
        }
        
        var split = trimmedLine.Split(' ', index, StringSplitOptions.RemoveEmptyEntries);

        var access = string.Empty;
        var special = string.Empty;
        var name = string.Empty;
        var args = string.Empty;
        var type = string.Empty;
        var contents = new List<string>();
        var overrideBool = split.Contains("override");

        switch (split.Length)
        {
            // (accessor/special) (type) (name/args)
            case 3:
            {
                if (CheckAccessModifier(split.First()))
                {
                    access = split.First();
                }
                else if (CheckSpecialModifier(split.First()))
                {
                    special = split.First();
                }
            
                var nameArgs = split.Last();
                var argsIndex = nameArgs.IndexOf('(');
                name = nameArgs.Substring(0, argsIndex);
                args = nameArgs.Substring(argsIndex);
                type = split[^2];
                break;
            }
            // (accessor) (special) (type) (name/args)
            case 4:
            {
                access = split[0];
                special = split[1];
            
                var nameArgs = split.Last();
                var argsIndex = nameArgs.IndexOf('(');
                name = nameArgs.Substring(0, argsIndex);
                args = nameArgs.Substring(argsIndex);
                type = split[^2];
                break;
            }
            // (accessor/special) (type/args) - Constructor
            case 2:
            {
                if (CheckAccessModifier(split.First()))
                {
                    access = split.First();
                }
                else if (CheckSpecialModifier(split.First()))
                {
                    special = split.First();
                }

                name = split.Last();
                break;
            }
            // (type/args) - Constructor
            case 1:
                name = split.First();
                break;
        }
        
        // Get contents
        if (!line.EndsWith(';'))
        {
            var content = _data.ElementAt(startIndex + 2);
            var contentIndex = startIndex + 2;
            while (content.TrimStart(' ') is not "}")
            {
                contents.Add(_data.ElementAt(contentIndex++).TrimStart(' '));
                content = _data.ElementAt(contentIndex);
            }
        }

        return (name, new(overrideBool, access, special, type, args, contents));
    }
    private (string, MemberModifiers) ParseMemberModifiers(string line)
    {
        var trimmedLine = line.TrimStart(' ');
        var index = trimmedLine.IndexOf('=');
        if (index > 0)
        {
            index = trimmedLine.Substring(0, index).Count(x => x is ' ') + 2;
        }

        var split = (index > 0) ? trimmedLine.Split(' ', index).ToList() : trimmedLine.Split(' ').ToList();
        
        var access = string.Empty;
        var special = string.Empty;
        var name = string.Empty;
        var type = string.Empty;
        var value = string.Empty;

        switch (split.Count)
        {
            // (accessor/special) (type) (name) = (value)
            case 5:
            {
                if (CheckAccessModifier(split.First()))
                {
                    access = split.First();
                }
                else if (CheckSpecialModifier(split.First()))
                {
                    special = split.First();
                }

                type = split[1];
                name = split[2];
                value = split[4];
                break;
            }
            // (accessor) (special) (type) (name) = (value)
            case 6:
                access = split[0];
                special = split[1];
                type = split[2];
                name = split[3];
                value = split[5];
                break;
            // (type) (name) = (value) or (accessor) (special) (type) (name)
            case 4:
                if (line.Contains('='))
                {
                    type = split[0];
                    name = split[1];
                    value = split[3];
                    break;
                }

                access = split[0];
                special = split[1];
                type = split[2];
                name = split[3];
                break;
            // (accessor/special) (type) (name)
            case 3:
                if (CheckAccessModifier(split.First()))
                {
                    access = split[0];
                }
                else if (CheckSpecialModifier(split.First()))
                {
                    special = split[0];
                }
                
                type = split[1];
                name = split[2];
                break;
            case 2:
                type = split[0];
                name = split[1];
                break;
        }

        return (name.TrimEnd(';'), new(access, special, type, value.TrimEnd(';')));
    }

    private (string, PropertyModifiers)? ParsePropertyModifiers(string line)
    {
        return null;
    }

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