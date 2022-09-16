using LanguageConvertor.Components;
using LanguageConvertor.Core;

const string lineData = "public static int Number = 0;";

var field = FieldComponent.Parse(lineData);
Console.WriteLine(field.Name);