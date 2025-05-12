namespace InnoClinic.Offices.Core.Abstractions;

/// <summary>
/// Represents a service for geocoding using the Yandex Geocoding API.
/// </summary>
public interface IYandexGeocodingService
{
    /// <summary>
    /// Asynchronously retrieves the longitude and latitude coordinates for a specified address.
    /// </summary>
    /// <param name="city">The city of the address.</param>
    /// <param name="street">The street of the address.</param>
    /// <param name="houseNumber">The house number of the address.</param>
    /// <returns>A tuple containing the longitude and latitude coordinates as strings.</returns>
    Task<(string longitude, string latitude)> GetCoordinatesAsync(string city, string street, string houseNumber);
}