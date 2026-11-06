using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OneOffice.Infrastructure.Persistence;

namespace OneOffice.Tests.Integration;

[Trait("Category", "Integration")]
public sealed class FoundationTests
{
    [Theory]
    [InlineData("Development", true, 200)]
    [InlineData("Development", false, 503)]
    [InlineData("Production", true, 200)]
    [InlineData("Production", false, 503)]
    public async Task Health_has_safe_body_and_liveness_is_independent(string environment, bool healthy, int status)
    {
        await using var factory = new ApiFactory(environment, healthy);
        using var client = Client(factory);
        var live = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        var ready = await client.GetAsync("/health/ready");
        Assert.Equal(status, (int)ready.StatusCode);
        Assert.Equal(healthy ? "{\"status\":\"healthy\"}" : "{\"status\":\"unavailable\"}", await ready.Content.ReadAsStringAsync());
        Assert.True(ready.Headers.CacheControl?.NoStore);
    }

    [Theory]
    [InlineData("Development", 200)]
    [InlineData("Production", 404)]
    public async Task OpenApi_is_environment_gated(string environment, int status)
    {
        await using var factory = new ApiFactory(environment);
        using var client = Client(factory);
        Assert.Equal(status, (int)(await client.GetAsync("/openapi/v1.json")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/_smoke/error")).StatusCode);
    }

    [Theory]
    [InlineData("Development", "/test-only/error", 500, "server.unexpected")]
    [InlineData("Production", "/test-only/error", 500, "server.unexpected")]
    [InlineData("Development", "/test-only/binding?number=TEST_SENTINEL", 400, "validation.failed")]
    [InlineData("Production", "/missing?password=TEST_SENTINEL", 404, "resource.not_found")]
    public async Task Errors_are_sanitized_and_correlated(string environment, string path, int status, string code)
    {
        await using var factory = new ApiFactory(environment);
        using var client = Client(factory);
        var response = await client.GetAsync(path);
        Assert.Equal(status, (int)response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("TEST_SENTINEL", body);
        Assert.DoesNotContain("StackTrace", body);
        var json = JsonDocument.Parse(body).RootElement;
        Assert.Equal(code, json.GetProperty("code").GetString());
        Assert.Equal(response.Headers.GetValues("X-Correlation-ID").Single(), json.GetProperty("traceId").GetString());
    }

    [Fact]
    public void Factory_forbids_resolving_real_database()
    {
        using var factory = new ApiFactory("Development");
        using var scope = factory.Services.CreateScope();
        Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<OneOfficeDbContext>());
    }

    private static HttpClient Client(ApiFactory factory) => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false
    });
}
