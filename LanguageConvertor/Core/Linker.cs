using LanguageConvertor.Components;

namespace LanguageConvertor.Core;

public enum ConvertibleLanguage
{
    Java,
    Cpp,
    Python
}

internal abstract class Linker
{
    protected readonly FilePack _filePack;
    protected readonly List<string> _formattedData;
    protected int _indentLevel = 0;

    protected Linker(string[] data)
    {
        var parser = new Parser(data);
        _filePack = parser.FilePack;
        _formattedData = new List<string>();
        _indentLevel = 0;
    }

    protected Linker(in FilePack filePack)
    {
        _filePack = filePack;
        _formattedData = new List<string>(_filePack.TotalCount);
        _indentLevel = 0;
    }

    protected abstract ConvertibleLanguage GetLanguage();
    protected abstract string GetImportKeyword();
    protected abstract string GetContainerKeyword();
    protected abstract string FormatImport(in ImportComponent importComponent);
    protected abstract string FormatContainer(in ContainerComponent containerComponent);
    protected abstract string FormatClass(in ClassComponent classComponent);
    protected abstract string FormatMethod(in MethodComponent methodComponent);
    protected abstract string FormatProperty(in PropertyComponent propertyComponent);
    protected abstract string FormatField(in FieldComponent fieldComponent);

    protected abstract void ConstructClass(List<ClassComponent> classes);
    protected abstract void ConstructMethods(List<MethodComponent> methods);
    protected abstract void ConstructFields(List<FieldComponent> fields);

    protected abstract void ConvertProperty(in ClassComponent classComponent);

    protected void Append(string text = "")
    {
        var indent = new string(' ', _indentLevel * 4);
        var data = $"{indent}{text}";
        _formattedData.Add(data);
    }
    protected void Append(char character)
    {
        var indent = new string(' ', _indentLevel * 4);
        var data = $"{indent}{character}";
        _formattedData.Add(data);
    }

    protected void IncrementIndent() => ++_indentLevel;
    protected void DecrementIndent() => --_indentLevel;
    protected string GetCurrentIndent() => new string(' ', _indentLevel * 4);

    public abstract IEnumerable<string> BuildFileLines();
    public abstract string BuildFile();

}
