using FluentValidation;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Application.Validators;
using InnoClinic.Offices.Core.Models.OfficeModels;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using InnoClinic.Offices.Core.Abstractions;

namespace InnoClinic.Offices.Application;

/// <summary>
/// Contains extension methods for adding services and FluentValidation to the service collection in the InnoClinic Offices application.
/// </summary>
public static class InnoClinicOfficesApplicationInjection
{
    /// <summary>
    /// Adds services related to geocoding, RabbitMQ, and office management to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IYandexGeocodingService, YandexGeocodingService>();
        services.AddScoped<IRabbitMQService, RabbitMQService>();
        services.AddScoped<IOfficeService, OfficeService>();

        return services;
    }

    /// <summary>
    /// Adds FluentValidation services to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the FluentValidation services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
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