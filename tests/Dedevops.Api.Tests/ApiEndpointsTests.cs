using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Dedevops.Api.Tests;

public sealed class ApiEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Health_ReturnsExpectedPayload()
    {
        var body = await _client.GetFromJsonAsync<HealthResponse>("/api/health");

        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
        Assert.Equal("dedevops-api", body.Application);
        Assert.Equal("1.0.0", body.Version);
    }

    [Fact]
    public async Task Info_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(body);
        Assert.Equal("dedevops-api", body.Application);
        Assert.False(string.IsNullOrWhiteSpace(body.Framework));
    }

    [Fact]
    public async Task UnknownEndpoint_ReturnsNotFoundAsProblemDetails()
    {
        var response = await _client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
