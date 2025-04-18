
namespace InnoClinic.Offices.Application.Services
{
    public interface IYandexGeocodingService
    {
        Task<(string longitude, string latitude)> GetCoordinatesAsync(string city, string street, string houseNumber);
    }
}