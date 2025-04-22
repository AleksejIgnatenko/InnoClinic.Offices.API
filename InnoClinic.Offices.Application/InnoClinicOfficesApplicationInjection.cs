using InnoClinic.Offices.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Offices.Application;

public static class InnoClinicOfficesApplicationInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IYandexGeocodingService, YandexGeocodingService>();
        services.AddScoped<IRabbitMQService, RabbitMQService>();
        services.AddTransient<IValidationService, ValidationService>();
        services.AddScoped<IOfficeService, OfficeService>();

        return services;
    }
}
