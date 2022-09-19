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

const string Path = @"C:\dev\LanguageConvertor\LanguageConvertor\Tests\Input\Data.txt";
var lines = File.ReadAllLines(Path);

//Parser parser = new Parser(lines);
JavaLinker javaLinker = new JavaLinker(lines);
var data = javaLinker.GetFormattedFileData();