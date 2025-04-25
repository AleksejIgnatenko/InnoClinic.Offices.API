using FluentValidation;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Application.Validators;
using InnoClinic.Offices.Core.Models.OfficeModels;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;

namespace InnoClinic.Offices.Application;

public static class InnoClinicOfficesApplicationInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IYandexGeocodingService, YandexGeocodingService>();
        services.AddScoped<IRabbitMQService, RabbitMQService>();
        services.AddScoped<IOfficeService, OfficeService>();

        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidator();

        services.AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection AddValidator(this IServiceCollection services)
    {
        services.AddScoped<IValidator<OfficeRequest>, OfficeRequestValidator>();

        return services;
    }
}