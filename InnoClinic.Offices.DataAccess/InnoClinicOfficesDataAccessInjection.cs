using InnoClinic.Offices.DataAccess.Context;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess;

public static class InnoClinicOfficesDataAccessInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(serviceProvidersp =>
        {
            var mongoDbSettings = serviceProvidersp.GetRequiredService<IOptions<MongoOptions>>().Value;
            return new MongoClient(mongoDbSettings.ConnectionUri);
        });

        services.AddSingleton<CustomMongoClient>();
        services.AddSingleton<MongoDbContext>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOfficeRepository, OfficeRepository>();

        return services;
    }
}
