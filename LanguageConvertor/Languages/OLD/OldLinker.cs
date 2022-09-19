using LanguageConvertor.Core;
using LanguageConvertor.Modifiers;

namespace LanguageConvertor.Languages;

public struct ContainerSettings
{
    public readonly string name;
    public readonly bool isFileScoped;

    public ContainerSettings(string name, bool isFileScoped)
    {
        this.name = name;
        this.isFileScoped = isFileScoped;
    }
}

public abstract class OldLinker
{
    protected IEnumerable<string> _data;

    protected List<string> _imports;
    protected ContainerSettings _container;
    protected Dictionary<string, ClassModifiers> _classModifers;
    protected Dictionary<string, MemberModifiers> _memberModifiers;
    protected Dictionary<string, MethodModifiers> _methodModifiers;

    public List<string> Imports => _imports;
    public ContainerSettings Container => _container;
    public Dictionary<string, ClassModifiers> Classes => _classModifers;
    public Dictionary<string, MemberModifiers> Members => _memberModifiers;
    public Dictionary<string, MethodModifiers> Methods => _methodModifiers;
    
    protected OldLinker(IEnumerable<string> data)
    {
        _data = data;

        var parser = new Parser(data);
        //_imports = parser.GetImports();
        //_container = parser.GetContainer(GetLanguage());
        //_classModifers = parser.GetClassModifiers();
        //_memberModifiers = parser.GetMemberModifiers();
        //_methodModifiers = parser.GetMethodModifiers();
    }

    protected abstract ConvertibleLanguage GetLanguage();
    protected abstract string GetContainerKeyword();
    protected abstract string GetImportKeyword();
    protected abstract string FormatContainer(string mainContainer);
    protected abstract string FormatImport(string importName);
    protected abstract string FormatInheritance(List<string> classes, List<string> interfaces);
    protected abstract string FormatClass(string className);
    protected abstract string FormatMethod(string methodName);
    protected abstract string FormatMember(string memberName);

    protected abstract void ConstructMethods(List<string> file, List<string> methods, ref int indentLevel);

    protected abstract void ConstructMembers(List<string> file, List<string> members, ref int indentLevel);

    protected string IncrementIndent(ref int indent)
    {
        ++indent;
        return new string(' ', indent * 4);
    }
    
    protected string DecrementIndent(ref int indent)
    {
        --indent;
        return new string(' ', indent * 4);
    }

    public abstract IEnumerable<string> GetFormattedFileData();
}