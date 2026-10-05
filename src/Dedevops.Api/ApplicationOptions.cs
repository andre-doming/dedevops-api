namespace Dedevops.Api;

public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    public string Name { get; set; } = "dedevops-api";

    public string Version { get; set; } = "1.0.0";

    public string Description { get; set; } = string.Empty;
}
