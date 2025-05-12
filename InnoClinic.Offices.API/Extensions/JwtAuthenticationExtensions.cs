using InnoClinic.Offices.Infrastructure.Options.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InnoClinic.Offices.API.Extensions;

/// <summary>
/// Contains extension methods for adding JWT authentication to the services collection.
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication to the services collection with the specified JWT options.
    /// </summary>
    /// <param name="services">The collection of services to add the authentication to.</param>
    /// <param name="jwtOptions">The JWT options configuration.</param>
    /// <returns>The updated collection of services.</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudiences = [jwtOptions.Value.Audience],
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Value.Issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey))
                };
            });

        return services;
    }
}