using LanguageConvertor.Components;
using LanguageConvertor.Core;
using LanguageConvertor.Languages;
using System.Linq;

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

const string Path = @"C:\dev\LanguageConvertor\LanguageConvertor\Tests\";
var lines = File.ReadAllLines($@"{Path}Input\Input.cs");

Convertor convertor = new Convertor($@"{Path}Input\Input.cs", ConvertibleLanguage.Java);
convertor.ToFile($@"{Path}\Output\");

//JavaLinker javaLinker = new JavaLinker(lines);
//var data = javaLinker.BuildFileLines();

/*var span = data.Replace("\r", "").AsSpan();

// Begin parse
while (span.Length > 0)
{
    var newLineIndex = span.IndexOf('\n');
    if (newLineIndex == -1) break;

    var buffer = span[..newLineIndex++];
    span = span[newLineIndex..];

    var currentString = buffer.Trim().ToString();
    if (string.IsNullOrEmpty(currentString) || currentString.Contains('{') || currentString.Contains('}'))
    {
        continue;
    }
    else
    {
        var method = MethodComponent.Parse(currentString);

        if (method.IsAbstract)
        {
            continue;
        }
        else
        {
            // Find the start of scope
            var bodySpan = span[1..];
            var scopeIndent = 0;
            while (scopeIndent >= 0)
            {
                var index = bodySpan.IndexOf('\n');
                var currentLine = bodySpan[..index++].TrimStart();

                if (currentLine.Contains('}')) scopeIndent--;

                Console.WriteLine($"{new string(' ', Math.Max(scopeIndent * 4, 0))}{currentLine}");
                bodySpan = bodySpan[index..];

                if (currentLine.Contains('{')) scopeIndent++;
            }

            var newIndex = span.Length - bodySpan.Length;
            span = span[++newIndex..];
        }
    }
}
*/
//Parser parser = new Parser(data);

//JavaLinker linker = new JavaLinker(lines);
//var data = linker.BuildFile();

//var span = data.AsSpan();

//File.WriteAllText($@"C:\Users\Brandon\IdeaProjects\Example\src\Output\FromCSharp.java", data);

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