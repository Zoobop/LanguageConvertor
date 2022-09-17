using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components
{
    internal struct MethodComponent : IComponent<MethodComponent>
    {
        public string? AccessModifier { get; set; }
        public string? SpecialModifier { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public IDictionary<string, string>? Parameters { get; }

        public bool IsStatic { get => SpecialModifier == "static"; }
        public bool IsPrivate { get => AccessModifier == "private" || AccessModifier == null; }
        public bool IsPublic { get => AccessModifier == "public"; }
        public bool IsProtected { get => AccessModifier == "protected"; }
        public bool HasParameters { get => Parameters != null && Parameters.Count > 0; }

        public MethodComponent(string? accessModifier, string? specialModifier, string? type, string? name, IDictionary<string, string>? parameters)
        {
            AccessModifier = accessModifier;
            SpecialModifier = specialModifier;
            Type = type;
            Name = name;
            Parameters = parameters;
        }

        public static MethodComponent Parse(string methodData)
        {
            var span = methodData.AsSpan();

            var accessor = string.Empty;
            var special = string.Empty;
            var parameters = new Dictionary<string, string>();

            // Try get accessor
            var hasAccess = span.StartsWith("public") || span.StartsWith("private") || span.StartsWith("protected");
            if (hasAccess)
            {
                var length = span.IndexOf(' ');
                accessor = span[..length++].ToString();
                //Console.WriteLine($"[{accessor}]");
                span = span[length..];
            }
            
            // Try get special
            var hasSpecial = span.StartsWith("static") || span.StartsWith("override") || span.StartsWith("virtual");
            if (hasSpecial)
            {
                var length = span.IndexOf(' ');
                special = span[..length++].ToString();
                //Console.WriteLine($"[{special}]");
                span = span[length..];
            }
            
            // Get type
            var typeIndex = span.IndexOf(' ');
            var type = span[..typeIndex++].ToString();
            //Console.WriteLine($"[{type}]");
            span = span[typeIndex..];

            // Get name
            var nameIndex = span.IndexOf('(');
            var name = span[..nameIndex].ToString();
            //Console.WriteLine($"[{name}]");
            span = span[nameIndex..];

            // Try get params
            var startParenthIndex = span.IndexOf('(');
            var endParenthIndex = span.IndexOf(')');
            if (endParenthIndex - startParenthIndex > 1)
            {
                span = span.TrimStart('(');
                var hasArgs = true;
                while (hasArgs)
                {
                    // Get arg type
                    var argTypeIndex = span.IndexOf(' ');
                    var argType = span[..argTypeIndex++].ToString();
                    span = span[argTypeIndex..];

                    // Get arg name
                    var tryArgIndex = span.IndexOf(',');
                    var argNameIndex = (tryArgIndex == -1) ? span.IndexOf(')') : tryArgIndex;
                    var argName = span[..argNameIndex++].ToString();
                    span = span[argNameIndex..].Trim();

                    //Console.WriteLine($"[{argType}:{argName}]");
                    parameters.Add(argName, argType);

                    // Break
                    if (tryArgIndex == -1) hasArgs = false;
                }
            }

            return new MethodComponent(accessor, special, type, name, parameters);
        }

        public override string ToString()
        {
            return $"[{AccessModifier}] [{SpecialModifier}] [{Type}] [{Name}] [{(Parameters != null ? string.Join(", ", Parameters) : string.Empty)}]";
        }
    }
}
