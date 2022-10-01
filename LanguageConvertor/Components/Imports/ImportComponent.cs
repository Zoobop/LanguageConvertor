namespace LanguageConvertor.Components;

public sealed class ImportComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
    public bool IsBuiltin { get; set; } = false;

    public ImportComponent()
    {
    }

    public ImportComponent(string name, bool isBuiltin)
    {
        Name = name;
        IsBuiltin = isBuiltin;
    }
}