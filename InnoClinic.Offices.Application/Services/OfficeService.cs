using AutoMapper;
using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Enums.Queues;

namespace InnoClinic.Offices.Application.Services;

/// <summary>
/// Service for managing office entities including creation, retrieval, updating, and deletion operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OfficeService"/> class.
/// </remarks>
/// <param name="officeRepository">The office repository for data access.</param>
/// <param name="rabbitMQService">The RabbitMQ service for message publishing.</param>
/// <param name="mapper">The mapper for object mapping.</param>
/// <param name="yandexGeocodingService">The Yandex Geocoding service for geocoding operations.</param>
public class OfficeService(IOfficeRepository officeRepository, IRabbitMQService rabbitMQService, IMapper mapper, IYandexGeocodingService yandexGeocodingService) : IOfficeService
{
    private readonly IOfficeRepository _officeRepository = officeRepository;
    private readonly IRabbitMQService _rabbitMQService = rabbitMQService;
    private readonly IMapper _mapper = mapper;
    private readonly IYandexGeocodingService _yandexGeocodingService = yandexGeocodingService;

    /// <summary>
    /// Creates a new office based on the provided office request.
    /// </summary>
    public async Task<OfficeEntity> CreateOfficeAsync(OfficeRequest officeRequest)
    {
        var office = _mapper.Map<OfficeEntity>(officeRequest);

        var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(office.City, office.Street, office.HouseNumber);
        office.Longitude = longitude;
        office.Latitude = latitude;

        await _officeRepository.CreateAsync(office);

        var officeDto = _mapper.Map<OfficeDto>(office);
        await _rabbitMQService.PublishMessageAsync(officeDto, OfficeQueuesEnum.AddOffice.ToString());

        return office;
    }

    /// <summary>
    /// Retrieves all offices.
    /// </summary>
    public async Task<IEnumerable<OfficeEntity>> GetAllOfficesAsync(CancellationToken cancellationToken)
    {
        return await _officeRepository.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all active offices.
    /// </summary>
    public async Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync(CancellationToken cancellationToken)
    {
        return await _officeRepository.GetByConditionAsync(office => office.IsActive == true, cancellationToken);
    }

    /// <summary>
    /// Retrieves an office by its unique identifier.
    /// </summary>
    public async Task<OfficeEntity> GetOfficeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _officeRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Updates an existing office based on the provided Id and office request.
    /// </summary>
    public async Task<OfficeEntity> UpdateOfficeAsync(Guid id, OfficeRequest officeRequest, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(id, cancellationToken);

        _mapper.Map(officeRequest, office);
        var (longitude, latitude) = await _yandexGeocodingService.GetCoordinatesAsync(office.City, office.Street, office.HouseNumber);
        office.Longitude = longitude;
        office.Latitude = latitude;

        await _officeRepository.UpdateAsync(office);

        var officeDto = _mapper.Map<OfficeDto>(office);
        await _rabbitMQService.PublishMessageAsync(officeDto, OfficeQueuesEnum.UpdateOffice.ToString());

        return office;
    }

    /// <summary>
    /// Deletes an office based on the provided Id.
    /// </summary>
    public async Task DeleteOfficeAsync(Guid id, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(id, cancellationToken);
        await _officeRepository.DeleteAsync(id);

        var officeDto = _mapper.Map<OfficeDto>(office);
        await _rabbitMQService.PublishMessageAsync(officeDto, OfficeQueuesEnum.DeleteOffice.ToString());
    }
}