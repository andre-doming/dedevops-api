namespace Dedevops.Api;

public sealed record HealthResponse(string Status, string Application, string Version);

public sealed record InfoResponse(
    string Application,
    string Version,
    string Description,
    string Environment,
    string Framework,
    string Hostname,
    DateTimeOffset TimestampUtc);
