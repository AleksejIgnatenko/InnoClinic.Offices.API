using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Exceptions;
using InnoClinic.Offices.Infrastructure.Options.YandexGeocoding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net;

namespace InnoClinic.Offices.Application.Services;

/// <summary>
/// Service for interacting with the Yandex Geocoding API to retrieve coordinates based on address information.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YandexGeocodingService"/> class.
/// </remarks>
/// <param name="yandexGeocodingOptions">The Yandex Geocoding API options.</param>
/// <param name="httpClient">The HTTP client used to make requests.</param>
public class YandexGeocodingService(IOptions<YandexGeocodingOptions> yandexGeocodingOptions, HttpClient httpClient) : IYandexGeocodingService
{
    private readonly YandexGeocodingOptions _yandexGeocodingOptions = yandexGeocodingOptions.Value;

    /// <summary>
    /// Asynchronously retrieves the latitude and longitude coordinates for a given address.
    /// </summary>
    /// <param name="city">The city of the address.</param>
    /// <param name="street">The street of the address.</param>
    /// <param name="houseNumber">The house number in the address.</param>
    /// <returns>A tuple containing the latitude and longitude coordinates as strings.</returns>
    public async Task<(string longitude, string latitude)> GetCoordinatesAsync(string city, string street, string houseNumber)
    {
        var url = $"{_yandexGeocodingOptions.YandexGeocodingApiUrl}?apikey={_yandexGeocodingOptions.ApiKey}&geocode={Uri.EscapeDataString(city + " " + street + " " + houseNumber)}&format=json";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(jsonResponse);

        var featureMember = json["response"]["GeoObjectCollection"]["featureMember"];
        if (featureMember != null && featureMember.HasValues)
        {
            var coordinates = featureMember[0]["GeoObject"]["Point"]["pos"].ToString().Split(' ');
            var longitude = coordinates[0];
            var latitude = coordinates[1];

            return (latitude, longitude);
        }

        throw new ExceptionWithStatusCode("The specified address could not be found", HttpStatusCode.NotFound);
    }
}