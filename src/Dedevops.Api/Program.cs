using System.Runtime.InteropServices;
using Dedevops.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(ApplicationOptions.SectionName));
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

var api = app.MapGroup("/api");

api.MapGet("/health", (IOptions<ApplicationOptions> options) =>
    Results.Ok(new HealthResponse("ok", options.Value.Name, options.Value.Version)));

api.MapGet("/info", (IOptions<ApplicationOptions> options, IHostEnvironment env) =>
    Results.Ok(new InfoResponse(
        Application: options.Value.Name,
        Version: options.Value.Version,
        Description: options.Value.Description,
        Environment: env.EnvironmentName,
        Framework: RuntimeInformation.FrameworkDescription,
        Hostname: Environment.MachineName,
        TimestampUtc: DateTimeOffset.UtcNow)));

app.Run();

public partial class Program;
