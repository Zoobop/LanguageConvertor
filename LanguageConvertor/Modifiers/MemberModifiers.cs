namespace LanguageConvertor.Modifiers;

public struct MemberModifiers
{
    public readonly string accessModifier;
    public readonly string specialModifier;
    public readonly string type;
    public readonly string value;

    public MemberModifiers(string accessModifier, string specialModifier, string type, string value)
    {
        this.accessModifier = accessModifier;
        this.type = type;
        this.value = value;
        this.specialModifier = specialModifier;
    }
}