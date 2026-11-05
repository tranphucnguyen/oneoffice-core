using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneOffice.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OneOfficeDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("OneOfficeDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Configure ConnectionStrings:OneOfficeDb using Development User Secrets.");
            }

            options.UseNpgsql(connectionString, postgres =>
                postgres.MigrationsAssembly(typeof(OneOfficeDbContext).Assembly.GetName().Name));
        });
        return services;
    }
}
