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
    protected readonly IEnumerable<string> _data;
    protected readonly Parser _parser;
    protected readonly List<string> _formattedData;
    protected int _indentLevel = 0;
    
    public Linker(IEnumerable<string> data)
    {
        _data = data;
        _parser = new Parser(data);
        _formattedData = new List<string>(_parser.TotalCount);
    }

    protected abstract ConvertibleLanguage GetLanguage();
    protected abstract string GetImportKeyword();
    protected abstract string GetContainerKeyword();
    protected abstract string FormatImport(string import);
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
        Console.WriteLine(data);
    }

    protected void IncrementIndent() => ++_indentLevel;
    protected void DecrementIndent() => --_indentLevel;

    public abstract IEnumerable<string> BuildFile();
}