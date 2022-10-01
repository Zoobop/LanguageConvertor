namespace LanguageConvertor.Components;

public sealed class ParameterComponent : IComponent
{
    public string Modifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ParameterComponent()
    {
    }

    public ParameterComponent(string modifier, string type, string name)
    {
        Modifier = modifier;
        Type = type;
        Name = name;
    }

    public static ParameterComponent ParseParameter(string parameterData)
    {
        
        
        return new ParameterComponent();
    }
}