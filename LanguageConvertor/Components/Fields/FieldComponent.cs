using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components
{
    internal struct FieldComponent : IComponent<FieldComponent>
    {
        public string? AccessModifier { get; set; }
        public string? SpecialModifier { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }

        public bool IsStatic { get => SpecialModifier == "static"; }
        public bool IsPrivate { get => AccessModifier == "private"; }
        public bool IsPublic { get => AccessModifier == "public"; }
        public bool IsProtected { get => AccessModifier == "protected"; }
        public bool HasValue { get => string.IsNullOrEmpty(Value); }

        public FieldComponent(string? accessModifier, string? specialModifier, string? type, string? name, string? value)
        {
            AccessModifier = accessModifier;
            SpecialModifier = specialModifier;
            Type = type;
            Name = name;
            Value = value;
        }

        public static FieldComponent Parse(string fieldLine)
        {
            var span = fieldLine.AsSpan();
            span.TrimEnd(';');

            var accessor = string.Empty;
            var special = string.Empty;
            var value = string.Empty;

            // Try get accessor
            var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
            if (hasAccess)
            {
                var length = span.IndexOf(' ');
                accessor = span[..length++].ToString();
                Console.WriteLine($"[{accessor}]");
                span = span[length..];
            }

            // Try get special
            var hasSpecial = span.StartsWith("static") || span.StartsWith("const");
            if (hasSpecial)
            {
                var length = span.IndexOf(' ');
                special = span[..length++].ToString();
                Console.WriteLine($"[{special}]");
                span = span[length..];
            }

            // Get type
            var typeIndex = span.IndexOf(' ');
            var type = span[..typeIndex++].ToString();
            Console.WriteLine($"[{type}]");
            span = span[typeIndex..];

            // Get name
            var tryIndex = span.IndexOf(' ');
            var nameIndex = (tryIndex == -1) ? span.IndexOf(';') : tryIndex;
            var name = span[..nameIndex++].ToString();
            Console.WriteLine($"[{name}]");
            span = span[nameIndex..];

            // Try get value
            var valueIndex = span.IndexOf('=');
            if (valueIndex != -1)
            {
                var index = valueIndex + 2; ;
                value = span[index..^1].ToString();
                Console.WriteLine($"[{value}]");
            }

            return new FieldComponent(accessor, special, type, name, value);
        }
    }
}
