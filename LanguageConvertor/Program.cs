using LanguageConvertor.Components;
using LanguageConvertor.Core;

using System.IO;

const string fieldData = "public static int Number = 0;";
const string propertyData = "public static int Number { get; private set; } = 0;";
const string methodData = "public virtual float Deg2Rad(float deg)";
const string classData = "public abstract class Test1 : BaseClass";

//var field = FieldComponent.Parse(fieldData);
//var property = PropertyComponent.Parse(propertyData);
//var method = MethodComponent.Parse(methodData);
var @class = ClassComponent.Parse(classData);
Console.WriteLine(@class);


/*
var fields = new List<FieldComponent>();
var properties = new List<PropertyComponent>();
var methods = new List<MethodComponent>();

const string Path = @"C:\dev\LanguageConvertor\LanguageConvertor\Tests\Input\Data.txt";
var lines = File.ReadAllLines(Path);
foreach (var line in lines)
{
    if (!line.Contains('{') && line.Contains('('))
    {
        // Methods
        methods.Add(MethodComponent.Parse(line));
    }
    else if (line.Contains('{'))
    {
        // Property
        properties.Add(PropertyComponent.Parse(line));
    }
    else
    {
        // Fields
        fields.Add(FieldComponent.Parse(line));
    }
}

Console.WriteLine("Fields");
foreach (var field in fields)
{
    Console.WriteLine(field);
}
Console.WriteLine();

Console.WriteLine("Property");
foreach (var property in properties)
{
    Console.WriteLine(property);
}
Console.WriteLine();

Console.WriteLine("Methods");
foreach (var method in methods)
{
    Console.WriteLine(method);
}
*/