using LanguageConvertor.Components;
using LanguageConvertor.Core;
using LanguageConvertor.Languages;
using System.Linq;
using System.Security.Claims;

//const string fieldData = "public static int Number = 0;";
//const string propertyData = "public static int Number { get; private set; } = 0;";
//const string methodData = "public virtual float Deg2Rad(float deg)";
//const string classData = "public abstract class Test1 : BaseClass";
//const string containerData = "namespace Container;";

//var field = FieldComponent.Parse(fieldData);
//var property = PropertyComponent.Parse(propertyData);
//var method = MethodComponent.Parse(methodData);
//var @class = ClassComponent.Parse(classData);
//var container = ContainerComponent.Parse(containerData);

const string Path = @"C:\dev\LanguageConvertor\LanguageConvertor\Tests\Input\Data.txt";
var lines = File.ReadAllLines(Path);

Parser parser = new Parser(lines);
JavaLinker linker = new JavaLinker(lines);
var data = linker.BuildFile();


/*// AGGREGATE
var allComponents = new List<IComponent>(parser.TotalCount);
allComponents.AddRange(parser.Containers.ConvertAll<IComponent>(x => x));
allComponents.AddRange(parser.Classes.ConvertAll<IComponent>(x => x));
allComponents.AddRange(parser.Methods.ConvertAll<IComponent>(x => x));
allComponents.AddRange(parser.Properties.ConvertAll<IComponent>(x => x));
allComponents.AddRange(parser.Fields.ConvertAll<IComponent>(x => x));

// FORMATTING
var format = new List<string>(allComponents.Count);

// Imports
foreach (var import in parser.Imports)
{
    Console.WriteLine($"import {import};");
}
Console.WriteLine();

foreach (var container in parser.Containers)
{
    allComponents.Remove(container);

    Console.WriteLine($"package {container.Name}");
    Console.WriteLine("{");

    foreach (var @class in container.Classes)
    {
        allComponents.Remove(@class);

        Console.WriteLine($"    class {@class.Name}");
        Console.WriteLine("    {");

        foreach (var field in @class.Fields)
        {
            Console.WriteLine($"        {field.Type} {field.Name};");
        }

        Console.WriteLine();

        foreach (var method in @class.Methods)
        {
            Console.WriteLine($"        {method.Type} {method.Name}");
            Console.WriteLine("        {");
            Console.WriteLine("        }");
            Console.WriteLine();
        }

        Console.WriteLine("    }");
    }

    Console.WriteLine("}");
}

if (allComponents.Count > 0)
{
    Console.WriteLine();

    var classes = allComponents
        .Where(x => x.GetType() == typeof(ClassComponent))
        .Select(x => (ClassComponent)x)
        .ToList();
    foreach (var classComponent in classes)
    {
        Console.WriteLine($"class {classComponent.Name}");
        Console.WriteLine("{");

        foreach (var field in classComponent.Fields)
        {
            Console.WriteLine($"    {field.Type} {field.Name};");
        }
        Console.WriteLine();

        foreach (var method in classComponent.Methods)
        {
            Console.WriteLine($"    {method.Type} {method.Name}");
            Console.WriteLine("    {");
            Console.WriteLine("    }");
            Console.WriteLine();
        }

        Console.WriteLine("}");
    }
}*/