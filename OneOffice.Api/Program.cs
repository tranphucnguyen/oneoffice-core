using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OneOffice.Api.Foundation;
using OneOffice.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
// Allowlisted structured events exclude raw framework/EF exceptions and HTTP payloads.
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Logging.AddFilter((category, level) =>
    category?.StartsWith("OneOffice.Api.Foundation", StringComparison.Ordinal) == true
    && level >= LogLevel.Information);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = ApiProblems.Create(context.HttpContext, 400);
        problem.Extensions["code"] = "validation.failed";
        problem.Type = "urn:oneoffice:problem:validation.failed";
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problem);
    };
});
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks().AddCheck<DatabaseReadiness>("database", tags: ["ready"], timeout: TimeSpan.FromSeconds(5));

var verifyDatabase = args.Contains("--verify-database", StringComparer.Ordinal);
if (verifyDatabase && !builder.Environment.IsDevelopment())
    throw new InvalidOperationException("Database verification requires Development.");
if (verifyDatabase) builder.Logging.ClearProviders();

var app = builder.Build();
if (verifyDatabase)
{
    Environment.ExitCode = await OneOffice.Api.Diagnostics.DatabaseVerification.RunAsync(app.Services);
    await app.DisposeAsync();
    return;
}

app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
    context.TraceIdentifier = traceId;
    context.Response.Headers["X-Correlation-ID"] = traceId;
    var started = Stopwatch.GetTimestamp();
    try { await next(context); }
    finally
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OneOffice.Api.Foundation.Request");
        logger.LogInformation("HTTP completed {StatusCode} {ElapsedMs} {TraceId}",
            context.Response.StatusCode, Stopwatch.GetElapsedTime(started).TotalMilliseconds, traceId);
    }
});
app.UseExceptionHandler(new ExceptionHandlerOptions { SuppressDiagnosticsCallback = _ => true });
app.UseStatusCodePages(async status =>
    await status.HttpContext.Response.WriteAsJsonAsync(ApiProblems.Create(status.HttpContext, status.HttpContext.Response.StatusCode),
        options: (System.Text.Json.JsonSerializerOptions?)null, contentType: "application/problem+json"));
if (app.Environment.IsDevelopment()) app.MapOpenApi();
// Smoke-only routes are opt-in and never mapped outside Development.
if (app.Environment.IsDevelopment() && args.Contains("--foundation-smoke", StringComparer.Ordinal))
{
    app.MapGet("/_smoke/error", () => Task.FromException<IResult>(new InvalidOperationException("Password=SMOKE_SECRET;Host=SMOKE_HOST")));
    app.MapGet("/_smoke/validation", (int number) => Results.Ok(number));
}
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = WriteHealth
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealth
});
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/health"), branch => branch.UseHttpsRedirection());
app.UseAuthorization();
app.MapControllers();
app.Run();

static Task WriteHealth(HttpContext context, HealthReport report)
{
    context.Response.Headers.CacheControl = "no-store";
    return context.Response.WriteAsJsonAsync(new { status = report.Status == HealthStatus.Healthy ? "healthy" : "unavailable" });
}
