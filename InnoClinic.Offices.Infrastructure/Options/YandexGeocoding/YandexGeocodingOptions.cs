namespace InnoClinic.Offices.Infrastructure.Options.YandexGeocoding;

/// <summary>
/// Represents the options for configuring Yandex Geocoding API settings.
/// </summary>
public class YandexGeocodingOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string YandexGeocodingApiUrl { get; set; } = string.Empty;
}