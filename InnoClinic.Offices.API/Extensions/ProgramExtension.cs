using InnoClinic.Offices.Application;
using InnoClinic.Offices.Application.MapperProfiles;
using InnoClinic.Offices.DataAccess;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Serilog;
using Microsoft.Extensions.Options;
using InnoClinic.Offices.API.Middlewares;
using FluentValidation.AspNetCore;
using InnoClinic.Offices.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using InnoClinic.Offices.Infrastructure.Options.Mongo;
using InnoClinic.Offices.Infrastructure.Options.Jwt;
using InnoClinic.Offices.Infrastructure.Options.RabbitMQ;
using InnoClinic.Offices.Infrastructure.Options.YandexGeocoding;
using InnoClinic.Offices.Infrastructure.Enums.Cache;

namespace InnoClinic.Offices.API.Extensions;

/// <summary>
/// Contains extension methods for configuring the web application builder and application startup.
/// </summary>
public static class ProgramExtension
{
    /// <summary>
    /// Configures the web application builder with necessary services and configurations.
    /// </summary>
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .CreateSerilog(builder.Host);

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        builder.Configuration
            .AddEnvironmentVariables()
            .LoadConfiguration();

        builder.Services
            .AddOptions(builder.Configuration)
            .AddDbContext()
            .AddRepositories()
            .AddServices()
            .AddCaching()
            .AddCustomSwagger()
            .AddEndpointsApiExplorer()
            .AddJwtAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>())
            .AddMapperProfiles()
            .AddFluentValidation()
            .AddHttpClient()
            .AddControllers();

        return builder;
    }

    /// <summary>
    /// Configures the web application with necessary middleware and services during startup.
    /// </summary>
    public static async Task<WebApplication> ConfigureApplicationAsync(this WebApplication app)
    {
        app.UseCustomExceptionHandler();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var rabbitMQService = services.GetRequiredService<IRabbitMQService>();
            await rabbitMQService.CreateQueuesAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static IConfiguration LoadConfiguration(this IConfigurationBuilder configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        return configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("yandexgeocodingsetting.json", optional: true, reloadOnChange: true)
            .Build();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
        services.Configure<YandexGeocodingOptions>(configuration.GetSection(nameof(YandexGeocodingOptions)));

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddResponseCaching(options =>
        {
            options.UseCaseSensitivePaths = true;
        });

        services.AddControllersWithViews(options =>
        {
            options.CacheProfiles.Add(CacheProfileNameEnum.CacheDefault90.ToString(), new CacheProfile
            {
                Duration = 90,
                Location = ResponseCacheLocation.Any
            });
        });

        return services;
    }

    private static IServiceCollection AddMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(OfficeMapperProfiles));

        return services;
    }

    private static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder webApplication)
    {
        webApplication.UseMiddleware<ExceptionHandlerMiddleware>();

        return webApplication;
    }
}