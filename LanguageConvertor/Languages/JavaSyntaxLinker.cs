using LanguageConvertor.Core;

namespace LanguageConvertor.Languages;

public class JavaSyntaxLinker : Linker
{
    public JavaSyntaxLinker(IEnumerable<string> data) : base(data)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Java;
    }

    protected override string GetContainerKeyword()
    {
        return "package";
    }

    protected override string GetImportKeyword()
    {
        return "import";
    }

    protected override string FormatContainer(string mainContainer)
    {
        return $"{GetContainerKeyword()} {mainContainer}";
    }

    protected override string FormatImport(string importName)
    {
        return $"{GetImportKeyword()} {importName};";
    }

    protected override string FormatInheritance(List<string> classes, List<string> interfaces)
    {
        var format = string.Empty;
        format += classes.Count > 0 ? $" extends {classes[0]}" : "";
        format += interfaces.Count > 0 ? $" implements {string.Join(", ", interfaces)}" : "";
        return format;
    }

    protected override string FormatClass(string className)
    {
        var modifiers = _classModifers[className];
        var access = string.IsNullOrEmpty(modifiers.accessModifier) ? "" : $"{modifiers.accessModifier} ";
        var special = string.IsNullOrEmpty(modifiers.specialModifier) ? "" : $"{modifiers.specialModifier} ";
        var inheritance = FormatInheritance(modifiers.inheritedClasses, modifiers.inheritedInterfaces);
        return $"{access}{special}class {className}{inheritance}";
    }

    protected override string FormatMethod(string methodName)
    {
        var modifiers = _methodModifiers[methodName];
        var access = string.IsNullOrEmpty(modifiers.accessModifier) ? "" : $"{modifiers.accessModifier} ";
        var special = string.IsNullOrEmpty(modifiers.specialModifier) || modifiers.specialModifier is "virtual" or "override" ? "" : $"{modifiers.specialModifier} ";
        var returnType = string.IsNullOrEmpty(modifiers.returnType) ? "" : $"{modifiers.returnType} ";
        var args = string.IsNullOrEmpty(modifiers.args) ? "" : modifiers.args;
        return $"{access}{special}{returnType}{methodName}{args}";
    }

    protected override string FormatMember(string memberName)
    {
        var modifiers = _memberModifiers[memberName];
        var access = string.IsNullOrEmpty(modifiers.accessModifier) ? "" : $"{modifiers.accessModifier} ";
        var special = string.IsNullOrEmpty(modifiers.specialModifier) || modifiers.specialModifier is "virtual" or "override" ? "" : $"{modifiers.specialModifier} ";
        var type = $"{modifiers.type} ";
        var assignment = string.IsNullOrEmpty(modifiers.value) ? "" : $" = {modifiers.value}";
        return $"{access}{special}{type}{memberName}{assignment};";
    }

    protected override void ConstructMethods(List<string> file, List<string> methods, ref int indentLevel)
    {
        throw new NotImplementedException();
    }

    protected override void ConstructMembers(List<string> file, List<string> members, ref int indentLevel)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetFormattedFileData()
    {
        // Get formatted imports
        var imports = new List<string>(Imports.Select(FormatImport));
        
        var file = new List<string>(imports) { "" };
        var indentLevel = 0;
        var indents = string.Empty;

        // Main Container
        if (!Container.isFileScoped)
        {
            file.Add(FormatContainer(Container.name));
            file.Add("{");
            indents = IncrementIndent(ref indentLevel);
        }
        else
        {
            file.Add($"{FormatContainer(Container.name)};");
            file.Add("");
        }

        // Class construction
        if (Classes.Count > 0)
        {
            file.Add($"{indents}{FormatClass(Classes.Keys.First())}");
            file.Add($"{indents}{{");
            indents = IncrementIndent(ref indentLevel);

            // MEMBERS
            file.AddRange(Members.Keys.Select(member => $"{indents}{FormatMember(member)}"));

            file.Add("");
            
            // METHODS
            foreach (var method in Methods.Keys)
            {
                if (Methods[method].overrideModifier)
                {
                    file.Add($"{indents}@Override");
                }

                file.Add($"{indents}{FormatMethod(method)}");
                file.Add($"{indents}{{");
                indents = IncrementIndent(ref indentLevel);

                file.AddRange(Methods[method].contents.Select(content => $"{indents}{content}"));
                
                indents = DecrementIndent(ref indentLevel);
                file.Add($"{indents}}}");
                file.Add("");
            }

            indents = DecrementIndent(ref indentLevel);
            file.Add($"{indents}}}");
        }

        if (!Container.isFileScoped)
        {
            indents = DecrementIndent(ref indentLevel);
            file.Add($"{indents}}}");
        }

        return file;
    }
}