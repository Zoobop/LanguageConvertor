using LanguageConvertor.Core;

namespace LanguageConvertor.Languages;

public class CppSyntaxLinker : Linker
{
    public CppSyntaxLinker(IEnumerable<string> data) : base(data)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Cpp;
    }

    protected override string GetContainerKeyword()
    {
        return "namespace";
    }

    protected override string GetImportKeyword()
    {
        return "#include";
    }

    protected override string FormatContainer(string mainContainer)
    {
        return $"{GetContainerKeyword()} {mainContainer}";
    }

    protected override string FormatImport(string importName)
    {
        return $"{GetImportKeyword()} \"{importName}\"";
    }

    protected override string FormatInheritance(List<string> classes, List<string> interfaces)
    {
        var parents = classes.Concat(interfaces).ToList();
        var format = parents.Count > 0 ? $" : public {string.Join(", public ", parents)}" : "";
        return format;
    }

    protected override string FormatClass(string className)
    {
        var modifiers = _classModifers[className];
        var inheritance = FormatInheritance(modifiers.inheritedClasses, modifiers.inheritedInterfaces);
        return $"class {className}{inheritance}";
    }

    protected override string FormatMethod(string methodName)
    {
        var modifiers = _methodModifiers[methodName];
        var overrideStr = modifiers.overrideModifier ? " override" : "";
        var special = string.IsNullOrEmpty(modifiers.specialModifier) || modifiers.specialModifier is "virtual" or "override" ? "" : $"{modifiers.specialModifier} ";
        var returnType = string.IsNullOrEmpty(modifiers.returnType) ? "" : $"{modifiers.returnType} ";
        var args = string.IsNullOrEmpty(modifiers.args) ? "" : modifiers.args;
        return $"{special}{returnType}{methodName}{args}{overrideStr}";
    }

    protected override string FormatMember(string memberName)
    {
        var modifiers = _memberModifiers[memberName];
        var special = string.IsNullOrEmpty(modifiers.specialModifier) || modifiers.specialModifier is "virtual" or "override" ? "" : $"{modifiers.specialModifier} ";
        var type = $"{modifiers.type} ";
        var assignment = string.IsNullOrEmpty(modifiers.value) ? "" : $" = {modifiers.value}";
        return $"{special}{type}{memberName}{assignment};";
    }

    protected override void ConstructMethods(List<string> file, List<string> methods, ref int indentLevel)
    {
        var indents = IncrementIndent(ref indentLevel);
                
        foreach (var method in methods)
        {
            var signature = $"{indents}{FormatMethod(method)}";
            if (signature.EndsWith(';'))
            {
                file.Add("");
                continue;
            }
                    
            file.Add(signature);
            file.Add($"{indents}{{");
            indents = IncrementIndent(ref indentLevel);
                    
            file.AddRange(Methods[method].contents.Select(line => $"{indents}{line}"));

            indents = DecrementIndent(ref indentLevel);
            file.Add($"{indents}}}");
            file.Add("");
        }

        indents = DecrementIndent(ref indentLevel);
    }

    protected override void ConstructMembers(List<string> file, List<string> members, ref int indentLevel)
    {
        var indents = IncrementIndent(ref indentLevel);

        file.AddRange(members.Select(member => $"{indents}{FormatMember(member)}"));

        indents = DecrementIndent(ref indentLevel);
    }
    
    public override IEnumerable<string> GetFormattedFileData()
    {
        // Get formatted imports
        var imports = new List<string>(Imports.Select(FormatImport));

        var file = new List<string> { "#pragma once", "" };
        file.AddRange(imports);
        file.Add("");
        
        var indentLevel = 0;
        
        // Main Container
        file.Add(FormatContainer(Container.name));
        file.Add("{");
        var indents = IncrementIndent(ref indentLevel);

        // Class construction
        var publicMethods = Methods.Keys.Where(x => Methods[x].accessModifier is "public").Distinct().ToList();
        var protectedMethods = Methods.Keys.Where(x => Methods[x].accessModifier is "protected").Distinct().ToList();
        var privateMethods = Methods.Keys.Where(x => Methods[x].accessModifier is "private").Distinct().ToList();

        var publicMembers = Members.Keys.Where(x => Members[x].accessModifier is "public").Distinct().ToList();
        var protectedMembers = Members.Keys.Where(x => Members[x].accessModifier is "protected").Distinct().ToList();
        var privateMembers = Members.Keys.Where(x => Members[x].accessModifier is "private").Distinct().ToList();
        
        file.Add($"{indents}{FormatClass(Classes.Keys.First())}");
        file.Add($"{indents}{{");

        // METHODS
        
        // Public methods
        if (publicMethods.Count > 0)
        {
            file.Add($"{indents}public:");
            ConstructMethods(file, publicMethods, ref indentLevel);
        }

        // Protected methods
        if (protectedMethods.Count > 0)
        {
            file.Add($"{indents}protected:");
            ConstructMethods(file, protectedMethods, ref indentLevel);
        }

        // Private methods
        if (privateMethods.Count > 0)
        {
            file.Add($"{indents}private:");
            ConstructMethods(file, privateMethods, ref indentLevel);
        }

        // MEMBERS
        
        // Public members
        if (publicMembers.Count > 0)
        {
            file.Add($"{indents}public:");
            ConstructMembers(file, publicMembers, ref indentLevel);
        }

        // Protected members
        if (protectedMembers.Count > 0)
        {
            file.Add($"{indents}protected:");
            ConstructMembers(file, protectedMembers, ref indentLevel);
        }

        // Private members
        if (privateMembers.Count > 0)
        {
            file.Add($"{indents}private:");
            ConstructMembers(file, privateMembers, ref indentLevel);
        }
        
        file.Add($"{indents}}}");
        
        indents = DecrementIndent(ref indentLevel);
        file.Add($"{indents}}}");
        
        return file;
    }
}