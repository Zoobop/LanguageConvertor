// See https://aka.ms/new-console-template for more information
using LanguageConvertor.Core;

var convertor = new Convertor(@"C:\dev\CSharp\LanguageConvertor\LanguageConvertor\Example.cs", ConvertibleLanguage.Cpp);

convertor.NewFile(@"C:\Users\Brandon\OneDrive\Desktop\Code\Converted");

/*var convertedData = convertor.GetConvertedData();

foreach (var line in convertedData)
{
    Console.WriteLine(line);
}*/