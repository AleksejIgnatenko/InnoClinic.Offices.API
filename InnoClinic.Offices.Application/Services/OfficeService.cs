using AutoMapper;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.RabbitMQ;

namespace InnoClinic.Offices.Application.Services;

public class OfficeService : IOfficeService
{ 
    private readonly IOfficeRepository _officeRepository;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IMapper _mapper;
    private readonly IYandexGeocodingService _yandexGeocodingService;

    public OfficeService(IOfficeRepository officeRepository, IRabbitMQService rabbitMQService, IMapper mapper, IYandexGeocodingService yandexGeocodingService)
    {
        _officeRepository = officeRepository;
        _rabbitMQService = rabbitMQService;
        _mapper = mapper;
        _yandexGeocodingService = yandexGeocodingService;
    }

    public async Task<OfficeEntity> CreateOfficeAsync(OfficeRequest officeRequest)
    {
        var office = _mapper.Map<OfficeEntity>(officeRequest);

        var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(office.City, office.Street, office.HouseNumber);
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

    public async Task UpdateOfficeAsync(Guid id, OfficeRequest officeRequest)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        _mapper.Map(officeRequest, office);
        var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(office.City, office.Street, office.HouseNumber);
        office.Longitude = longitude;
        office.Latitude = latitude;

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
