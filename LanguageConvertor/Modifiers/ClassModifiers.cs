namespace LanguageConvertor.Modifiers;

public struct ClassModifiers
{
    public readonly string accessModifier;
    public readonly string specialModifier;
    public readonly List<string> inheritedClasses;
    public readonly List<string> inheritedInterfaces;

    public ClassModifiers(string accessModifier, string specialModifier, List<string> inheritance)
    {
        this.accessModifier = accessModifier;
        this.specialModifier = specialModifier;
        this.inheritedClasses = new();
        this.inheritedInterfaces = new();

        foreach (var parent in inheritance)
        {
            if (parent.StartsWith('I'))
            {
                inheritedInterfaces.Add(parent);
                continue;
            }
            inheritedClasses.Add(parent);
        }
    }
}