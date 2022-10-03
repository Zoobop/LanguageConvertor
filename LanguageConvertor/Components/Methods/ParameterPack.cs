namespace LanguageConvertor.Components;

public sealed class ParameterPack
{
    public string Modifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ParameterPack()
    {
    }

    public ParameterPack(string name, string type, string modifier = "")
    {
        Modifier = modifier;
        Type = type;
        Name = name;
    }
}