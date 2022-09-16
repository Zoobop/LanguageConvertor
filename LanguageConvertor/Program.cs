using LanguageConvertor.Components;
using LanguageConvertor.Core;

const string fieldData = "public static int Number = 0;";
const string propertyData = "public static int Number { get; private set; } = 0;";
const string methodData = "public static void Method(string arg1, int arg2)";

//var field = FieldComponent.Parse(fieldData);
//var property = PropertyComponent.Parse(propertyData);
//var method = MethodComponent.Parse(methodData);

/*var fields = new List<FieldComponent>();
var properties = new List<PropertyComponent>();
var methods = new List<MethodComponent>();*/