using AutoMapper;
using InnoClinic.Offices.Core.Dto;
using InnoClinic.Offices.Core.Exceptions;
using InnoClinic.Offices.Core.Models;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.RabbitMQ;
using Newtonsoft.Json.Linq;

namespace InnoClinic.Offices.Application.Services
{
    public class OfficeService : IOfficeService
    {
        private const string API_Key = "7f22f510-13a1-4bb2-bd1d-65a4b2e24ea6";
        private const string YANDEX_GEOCODING_API_URL = "https://geocode-maps.yandex.ru/1.x/";
 
        private readonly HttpClient _httpClient;
        private readonly IOfficeRepository _officeRepository;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IMapper _mapper;

        public OfficeService(IOfficeRepository officeRepository, HttpClient httpClient, IRabbitMQService rabbitMQService, IMapper mapper)
        {
            _officeRepository = officeRepository;
            _httpClient = httpClient;
            _rabbitMQService = rabbitMQService;
            _mapper = mapper;
        }

        public async Task CreateOfficeAsync(string address, Guid photoId, string registryPhoneNumber, bool isActive)
        {
            var (longitude, latitude) = await GetCoordinatesAsync(address);

            var office = new OfficeModel
            {
                Id = Guid.NewGuid(),
                Address = address,
                Longitude = longitude,
                Latitude = latitude,
                PhotoId = photoId,
                RegistryPhoneNumber = registryPhoneNumber,
                IsActive = isActive
            };

            await _officeRepository.CreateAsync(office);

            var officeDto = _mapper.Map<OfficeDto>(office);
            await _rabbitMQService.PublishMessageAsync(officeDto, RabbitMQQueues.ADD_OFFICE_QUEUE);
        }

        public async Task<IEnumerable<OfficeModel>> GetAllOfficesAsync()
        {
            return await _officeRepository.GetAllAsync();
        }

        public async Task<OfficeModel> GetOfficeByIdAsync(Guid id)
        {
            return await _officeRepository.GetByIdAsync(id);
        }

        public async Task UpdateOfficeAsync(string address, Guid photoId, string registryPhoneNumber, bool isActive)
        {
            var office = new OfficeModel
            {
                Id = Guid.NewGuid(),
                Address = address,
                PhotoId = photoId,
                RegistryPhoneNumber = registryPhoneNumber,
                IsActive = isActive
            };

            await _officeRepository.UpdateAsync(office);
        }

        public async Task DeleteOfficeAsync(Guid id)
        {
            await _officeRepository.DeleteAsync(id);
        }

        private async Task<(string longitude, string latitude)> GetCoordinatesAsync(string address)
        {
            var url = $"{YANDEX_GEOCODING_API_URL}?apikey={API_Key}&geocode={Uri.EscapeDataString(address)}&format=json";

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

            throw new DataRepositoryException("The specified address could not be found", 404);
        }
    }
}
