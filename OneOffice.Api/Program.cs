using OneOffice.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var verifyDatabase = args.Contains("--verify-database", StringComparer.Ordinal);
if (verifyDatabase)
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException("Database verification requires Development.");
    builder.Logging.ClearProviders();
}

var app = builder.Build();
if (verifyDatabase)
{
    Environment.ExitCode = await OneOffice.Api.Diagnostics.DatabaseVerification.RunAsync(app.Services);
    await app.DisposeAsync();
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
