using InnoClinic.Offices.Core.Exceptions;
using InnoClinic.Offices.Infrastructure.YandexGeocoding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace InnoClinic.Offices.Application.Services;

public class YandexGeocodingService : IYandexGeocodingService
{
    private readonly YandexGeocodingOptions _yandexGeocodingOptions;
    private readonly HttpClient _httpClient;

    public YandexGeocodingService(IOptions<YandexGeocodingOptions> yandexGeocodingOptions, HttpClient httpClient)
    {
        _yandexGeocodingOptions = yandexGeocodingOptions.Value;
        _httpClient = httpClient;
    }

    public async Task<(string longitude, string latitude)> GetCoordinatesAsync(string city, string street, string houseNumber)
    {
        var url = $"{_yandexGeocodingOptions.YandexGeocodingApiUrl}?apikey={_yandexGeocodingOptions.ApiKey}&geocode={Uri.EscapeDataString(city + " " + street + " " + houseNumber)}&format=json";

        var response = await _httpClient.GetAsync(url);
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

        throw new ExceptionWithStatusCode("The specified address could not be found", 404);
    }
}
