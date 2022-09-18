using LanguageConvertor.Components;
using LanguageConvertor.Core;

using System.IO;

const string fieldData = "public static int Number = 0;";
const string propertyData = "public static int Number { get; private set; } = 0;";
const string methodData = "public virtual float Deg2Rad(float deg)";
const string classData = "public abstract class Test1 : BaseClass";
const string containerData = "namespace Container;";

//var field = FieldComponent.Parse(fieldData);
//var property = PropertyComponent.Parse(propertyData);
//var method = MethodComponent.Parse(methodData);
//var @class = ClassComponent.Parse(classData);
//var container = ContainerComponent.Parse(containerData);
//Console.WriteLine(container);



const string Path = @"C:\dev\LanguageConvertor\LanguageConvertor\Tests\Input\Input.cs";
var lines = File.ReadAllLines(Path);

Parser parser = new Parser(lines);



/*
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