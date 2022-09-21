using LanguageConvertor.Languages;

namespace LanguageConvertor.Core;

public sealed class Convertor
{
    private readonly string _filePath;
    private readonly string[] _fileData;
    private readonly ConvertibleLanguage _language;

    internal Linker Linker { get; }

    public Convertor(string filePath, ConvertibleLanguage language)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Could not find file!");
        }

        _filePath = filePath;
        _language = language;
        _fileData = File.ReadAllLines(filePath);

        Linker = language switch
        {
            ConvertibleLanguage.Cpp => new CppLinker(_fileData),
            _ => new JavaLinker(_fileData)
        };
    }
    
    public Convertor(string[] data, ConvertibleLanguage language)
    {
        _filePath = string.Empty;
        _language = language;
        _fileData = data;

        Linker = language switch
        {
            ConvertibleLanguage.Cpp => new CppLinker(_fileData),
            _ => new JavaLinker(_fileData)
        };
    }

    public IEnumerable<string> GetDataAsLines()
    {
        return Linker.BuildFileLines();
    }

    public string GetData()
    {
        return Linker.BuildFile();
    }

    public void ToFile(string exportPath)
    {
        var span = _filePath.AsSpan();
        var startIndex = span.LastIndexOf('\\');
        var endIndex = span.LastIndexOf('.');
        var fileName = span[startIndex..endIndex].ToString();
        var extension = _language switch
        {
            ConvertibleLanguage.Cpp => ".hpp",
            _ => ".java"
        };

        var convertedData = GetDataAsLines();
        var completePath = $"{exportPath}{fileName}{extension}";
        File.WriteAllLines(completePath, convertedData);
    } 
}