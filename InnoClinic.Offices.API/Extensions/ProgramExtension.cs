using InnoClinic.Offices.Application;
using InnoClinic.Offices.Application.MapperProfiles;
using InnoClinic.Offices.DataAccess;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Serilog;
using InnoClinic.Offices.Infrastructure.YandexGeocoding;
using InnoClinic.Offices.Infrastructure.Mongo;
using InnoClinic.Offices.Infrastructure.RabbitMQ;
using InnoClinic.Offices.Infrastructure.Jwt;
using InnoClinic.Offices.Application.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using InnoClinic.Offices.Infrastructure.Cors;
using Microsoft.Extensions.Options;
using InnoClinic.Offices.API.Middlewares;
using FluentValidation.AspNetCore;

namespace InnoClinic.Offices.API.Extensions;

public static class ProgramExtension
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .CreateSerilog(builder.Host);

        builder.Configuration
            .AddEnvironmentVariables()
            .LoadConfiguration();

        builder.Services
            .AddOptions(builder.Configuration)
            .AddDbContext()
            .AddServices()
            .AddRepositories()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpClient()
            .AddJwtAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>())
            .AddCors(options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var customCorsOptions = serviceProvider.GetRequiredService<IOptions<CustomCorsOptions>>();
                options.ConfigureAllowAllCors(customCorsOptions);
            })
            .AddFluentValidation()
            .AddControllers();
        
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        builder.Services.AddAutoMapper(typeof(OfficeMapperProfiles));

        return builder;
    }

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

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseCors("AllowAll");
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

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
        services.Configure<YandexGeocodingOptions>(configuration.GetSection(nameof(YandexGeocodingOptions)));
        services.Configure<CustomCorsOptions>(configuration.GetSection(nameof(CustomCorsOptions)));

        return services;
    }

    private static CorsOptions ConfigureAllowAllCors(this CorsOptions options, IOptions<CustomCorsOptions> customCorsOptions)
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithHeaders().AllowAnyHeader();
            policy.WithOrigins(customCorsOptions.Value.AllowedOrigins);
            policy.WithMethods().AllowAnyMethod();
            policy.AllowCredentials();
        });

        return options;
    }

    private static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder webApplication)
    {
        webApplication.UseMiddleware<ExceptionHandlerMiddleware>();

        return webApplication;
    }
}
