namespace LanguageConvertor.Modifiers;

public struct MethodModifiers
{
    public readonly bool overrideModifier;
    public readonly string specialModifier;
    public readonly string accessModifier;
    public readonly string returnType;
    public readonly string args;
    public readonly List<string> contents;

    public MethodModifiers(bool overrideModifier, string accessModifier,  string specialModifier, string returnType, string args, List<string> contents)
    {
        this.overrideModifier = overrideModifier;
        this.accessModifier = accessModifier;
        this.returnType = returnType;
        this.args = args;
        this.contents = contents;
        this.specialModifier = specialModifier;
    }
}