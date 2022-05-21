using LanguageConvertor.Core;

namespace LanguageConvertor.Languages;

public class PythonSyntaxLinker : Linker
{
    public PythonSyntaxLinker(IEnumerable<string> data) : base(data)
    {
    }

    protected override ConvertibleLanguage GetLanguage()
    {
        return ConvertibleLanguage.Python;
    }

    protected override string GetContainerKeyword()
    {
        return "";
    }

    protected override string GetImportKeyword()
    {
        return "import";
    }

    protected override string FormatContainer(string mainContainer)
    {
        return "";
    }

    protected override string FormatImport(string importName)
    {
        throw new NotImplementedException();
    }

    protected override string FormatInheritance(List<string> classes, List<string> interfaces)
    {
        throw new NotImplementedException();
    }

    protected override string FormatClass(string className)
    {
        throw new NotImplementedException();
    }

    protected override string FormatMethod(string methodName)
    {
        throw new NotImplementedException();
    }

    protected override string FormatMember(string memberName)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}