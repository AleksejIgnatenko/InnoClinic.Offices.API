using AutoMapper;
using FluentValidation;
using InnoClinic.Offices.Core.Exceptions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.RabbitMQ;

namespace InnoClinic.Offices.Application.Services
{
    public class OfficeService : IOfficeService
    { 
        private readonly IOfficeRepository _officeRepository;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private readonly IYandexGeocodingService _yandexGeocodingService;

        public OfficeService(IOfficeRepository officeRepository, IRabbitMQService rabbitMQService, IMapper mapper, IValidationService validationService, IYandexGeocodingService yandexGeocodingService)
        {
            _officeRepository = officeRepository;
            _rabbitMQService = rabbitMQService;
            _mapper = mapper;
            _validationService = validationService;
            _yandexGeocodingService = yandexGeocodingService;
        }

        public async Task<OfficeEntity> CreateOfficeAsync(string city, string street, string houseNumber, string officeNumber, string? photoId, string registryPhoneNumber, bool isActive)
        {

            var office = new OfficeEntity
            {
                Id = Guid.NewGuid(),
                City = city,
                Street = street,
                HouseNumber = houseNumber,
                OfficeNumber = officeNumber,
                Longitude = string.Empty,
                Latitude = string.Empty,
                PhotoId = photoId,
                RegistryPhoneNumber = registryPhoneNumber,
                IsActive = isActive
            };

            var validationErrors = _validationService.Validation(office);

            if (validationErrors.Count != 0)
            {
                throw new ValidationException(validationErrors);
            }

            var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(city, street, houseNumber);
            office.Longitude = longitude;
            office.Latitude = latitude;

            await _officeRepository.CreateAsync(office);

            var officeDto = _mapper.Map<OfficeDto>(office);
            await _rabbitMQService.PublishMessageAsync(officeDto, RabbitMQQueues.ADD_OFFICE_QUEUE);

            return office;
        }

        public async Task<IEnumerable<OfficeEntity>> GetAllOfficesAsync()
        {
            return await _officeRepository.GetAllAsync();
        }

        public async Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync()
        {
            return await _officeRepository.GetAllActiveOfficesAsync();
        }

        public async Task<OfficeEntity> GetOfficeByIdAsync(Guid id)
        {
            return await _officeRepository.GetByIdAsync(id);
        }

        public async Task UpdateOfficeAsync(Guid id, string city, string street, string houseNumber, string officeNumber, string? photoId, string registryPhoneNumber, bool isActive)
        {
            var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(city, street, houseNumber);

            var office = new OfficeEntity
            {
                Id = id,
                City = city,
                Street = street,
                HouseNumber = houseNumber,
                OfficeNumber = officeNumber,
                Longitude = longitude,
                Latitude = latitude,
                PhotoId = photoId,
                RegistryPhoneNumber = registryPhoneNumber,
                IsActive = isActive
            };

            await _officeRepository.UpdateAsync(office);

            var officeDto = _mapper.Map<OfficeDto>(office);
            await _rabbitMQService.PublishMessageAsync(officeDto, RabbitMQQueues.UPDATE_OFFICE_QUEUE);
        }

        public async Task DeleteOfficeAsync(Guid id)
        {
            var office = await _officeRepository.GetByIdAsync(id);
            await _officeRepository.DeleteAsync(id);

            var officeDto = _mapper.Map<OfficeDto>(office);
            await _rabbitMQService.PublishMessageAsync(officeDto, RabbitMQQueues.DELETE_OFFICE_QUEUE);
        }
    }
}
