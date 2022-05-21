using LanguageConvertor.Languages;

namespace LanguageConvertor.Core;

public class Convertor
{
    private readonly string _filePath;
    private readonly IEnumerable<string> _fileData;
    private readonly ConvertibleLanguage _language;

    public Linker Linker { get; }

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
            ConvertibleLanguage.Cpp => new CppSyntaxLinker(_fileData),
            _ => new JavaSyntaxLinker(_fileData)
        };
    }
    
    public Convertor(IEnumerable<string> data, ConvertibleLanguage language)
    {
        _filePath = string.Empty;
        _language = language;
        _fileData = data;

        Linker = language switch
        {
            ConvertibleLanguage.Cpp => new CppSyntaxLinker(_fileData),
            _ => new JavaSyntaxLinker(_fileData)
        };
    }

    public IEnumerable<string> GetConvertedData()
    {
        return Linker.GetFormattedFileData();
    }

    public void NewFile(string exportPath)
    {
        var startIndex = _filePath.LastIndexOf('\\');
        var endIndex = _filePath.LastIndexOf('.');
        var fileName = _filePath.Substring(startIndex, endIndex - startIndex);
        var extension = _language switch
        {
            ConvertibleLanguage.Cpp => ".hpp",
            _ => ".java"
        };

        var convertedData = GetConvertedData();
        
        var completePath = $"{exportPath}{fileName}{extension}";
        File.WriteAllLines(completePath, convertedData);
    } 
}