using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OneOffice.Infrastructure.Persistence;

namespace OneOffice.Tests.Integration;

internal sealed class ApiFactory(string environment, bool healthy = true) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.ConfigureAppConfiguration((_, configuration) => configuration.Sources.Clear());
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<OneOfficeDbContext>();
            services.AddScoped<OneOfficeDbContext>(_ => throw new InvalidOperationException("Database access forbidden in HTTP tests."));
            services.PostConfigure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
                options.Registrations.Add(new HealthCheckRegistration("controlled", _ => new ControlledHealth(healthy), null, ["ready"]));
            });
            services.AddControllers().AddApplicationPart(typeof(FailureController).Assembly);
        });
    }

    private sealed class ControlledHealth(bool healthy) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            => Task.FromResult(healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("Password=TEST_SENTINEL"));
    }
}

[ApiController]
[Route("test-only")]
public sealed class FailureController : ControllerBase
{
    [HttpGet("error")]
    public IActionResult Error() => throw new InvalidOperationException("Password=TEST_SENTINEL");

    [HttpGet("binding")]
    public IActionResult Binding([FromQuery] int number) => Ok(number);
}
