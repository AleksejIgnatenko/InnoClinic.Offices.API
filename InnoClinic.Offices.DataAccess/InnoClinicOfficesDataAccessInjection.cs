using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.DataAccess.Context;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.Options.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess;

/// <summary>
/// Provides extension methods for registering data access dependencies related to InnoClinic offices.
/// </summary>
public static class InnoClinicOfficesDataAccessInjection
{
    /// <summary>
    /// Adds the MongoDB context and related services to the service collection.
    /// </summary>
    /// <param name="services">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoOptions>>().Value;
            return new MongoClient(mongoDbSettings.ConnectionUri);
        });

        services.AddSingleton<CustomMongoClient>();
        services.AddSingleton<MongoDbContext>();

        return services;
    }

    /// <summary>
    /// Adds repositories to the service collection.
    /// </summary>
    /// <param name="services">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOfficeRepository, OfficeRepository>();

        return services;
    }
}