namespace LanguageConvertor.Modifiers;

public struct PropertyModifiers
{
    public readonly string accessModifier;
    public readonly string specialModifier;
    public readonly string type;
    public readonly string value;
    public readonly bool getter;
    public readonly bool setter;
    public readonly string setterAccessModifier;

    public PropertyModifiers(string accessModifier, string specialModifier, string type, string value, bool getter, bool setter, string setterAccessModifier)
    {
        this.accessModifier = accessModifier;
        this.type = type;
        this.value = value;
        this.specialModifier = specialModifier;
        this.getter = getter;
        this.setter = setter;
        this.setterAccessModifier = setterAccessModifier;
    }
}