using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senai.TCC.Catalog.Domain.Repositories;
using Senai.TCC.Catalog.Persistence.Postgres.EFCore.Repositories;

namespace Senai.TCC.Catalog.Persistence.Postgres.EFCore;

public static class PostgresPersistenceIoCConfiguratorHelper
{
    public static IServiceCollection AddPostgresCatalogDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDbContext<PostgresCatalogDbContext>(options =>
            {
                options.UseNpgsql(configuration["PostgresDbContextSettings:ConnectionString"],
                    npgsqlOptionsAction: postgresOptions =>
                    {
                        postgresOptions.MigrationsAssembly(typeof(PostgresCatalogDbContext).Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        postgresOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: new List<string> { "Migrations failed" });
                    });
            }, ServiceLifetime.Singleton, ServiceLifetime.Transient);

        services.AddScoped<ICatalogRepository, PostgresCatalogRepository>();
        return services;
    }
}