﻿using AutoMapper;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.RabbitMQ;
using Moq;

namespace InnoClinic.Offices.TestSuiteNUnit.ServiceTests;

class OfficeServiceTests
{
    private Mock<IOfficeRepository> _officeRepositoryMock;
    private Mock<IRabbitMQService> _rabbitMQServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IYandexGeocodingService> _yandexGeocodingServiceMock;

    private OfficeService _officeService;
    private OfficeEntity officeEntity;

    private OfficeDto officeDto;
    private OfficeRequest officeRequest;

    [SetUp]
    public void SetUp()
    {
        officeRequest = new OfficeRequest("City", "Street", "HouseNumber", "OfficeNumber", "PhotoId", "RegistryPhoneNumber", true);
        officeEntity = new OfficeEntity
        {
            Id = Guid.NewGuid(),
            City = "City",
            Street = "Street",
            HouseNumber = "HouseNumber",
            OfficeNumber = "OfficeNumber",
            PhotoId = "PhotoId",
            RegistryPhoneNumber = "RegistryPhoneNumber",
            IsActive = true,
        };
        officeDto = new OfficeDto(officeEntity.Id, officeEntity.City, officeEntity.Street, officeEntity.HouseNumber, officeEntity.OfficeNumber, officeEntity.RegistryPhoneNumber, officeEntity.IsActive);

        _officeRepositoryMock = new Mock<IOfficeRepository>();
        _rabbitMQServiceMock = new Mock<IRabbitMQService>();
        _mapperMock = new Mock<IMapper>();
        _yandexGeocodingServiceMock = new Mock<IYandexGeocodingService>();

        _officeService = new OfficeService(
            _officeRepositoryMock.Object,
            _rabbitMQServiceMock.Object,
            _mapperMock.Object,
            _yandexGeocodingServiceMock.Object
            );
    }

    [Test]
    public async Task CreateOfficeAsync_ValidOffice_ResultCreated()
    {
        // Arrange
        _mapperMock.Setup(m => m.Map<OfficeEntity>(It.IsAny<OfficeRequest>())).Returns(officeEntity);
        _yandexGeocodingServiceMock.Setup(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("longitude", "latitude"));
        _officeRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<OfficeEntity>())).Returns(Task.FromResult(officeEntity));
        _mapperMock.Setup(m => m.Map<OfficeDto>(officeEntity)).Returns(officeDto);

        // Act
        await _officeService.CreateOfficeAsync(officeRequest);

        // Assert
        _mapperMock.Verify(m => m.Map<OfficeEntity>(officeRequest), Times.Once);
        _yandexGeocodingServiceMock.Verify(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(officeEntity), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, RabbitMQQueues.ADD_OFFICE_QUEUE), Times.Once);
    }

    [Test]
    public async Task GetAllOfficesAsync_ShouldReturnAllOffices()
    {
        // Arrange
        var officeEntities = new List<OfficeEntity>
        {
            new OfficeEntity { Id = Guid.NewGuid() }
        };

        officeEntities.Add(officeEntity);

        _officeRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(officeEntities);

        // Act
        var result = await _officeService.GetAllOfficesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(officeEntities.Count, result.Count());

        _officeRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllActiveOfficesAsync_ShouldReturnAllActiveOffices()
    {
        // Arrange
        var officeEntities = new List<OfficeEntity>
        {
            new OfficeEntity { Id = Guid.NewGuid(), IsActive = true }
        };

        officeEntities.Add(officeEntity);

        _officeRepositoryMock.Setup(repo => repo.GetAllActiveOfficesAsync()).ReturnsAsync(officeEntities);

        // Act
        var result = await _officeService.GetAllActiveOfficesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(officeEntities.Count, result.Count());

        _officeRepositoryMock.Verify(repo => repo.GetAllActiveOfficesAsync(), Times.Once);
    }

    [Test]
    public async Task GetOfficeByIdAsync_ShouldReturnOfficeById()
    {
        // Arrange
        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeEntity.Id)).ReturnsAsync(officeEntity);

        // Act
        var result = await _officeService.GetOfficeByIdAsync(officeEntity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(officeEntity.Id, result.Id);

        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeEntity.Id), Times.Once);
    }

    [Test]
    public async Task UpdateOfficeAsync_ValidOffice_ResultUpdate()
    {
        // Arrange
        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeEntity.Id)).ReturnsAsync(officeEntity);
        _mapperMock.Setup(m => m.Map<OfficeEntity>(It.IsAny<OfficeRequest>())).Returns(officeEntity);
        _yandexGeocodingServiceMock.Setup(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("longitude", "latitude"));
        _officeRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<OfficeEntity>())).Returns(Task.FromResult(officeEntity));
        _mapperMock.Setup(m => m.Map<OfficeDto>(officeEntity)).Returns(officeDto);

        // Act
        await _officeService.UpdateOfficeAsync(officeEntity.Id, officeRequest);

        // Assert
        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeEntity.Id), Times.Once);
        _mapperMock.Verify(m => m.Map(officeRequest, officeEntity), Times.Once);
        _yandexGeocodingServiceMock.Verify(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(officeEntity), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, RabbitMQQueues.UPDATE_OFFICE_QUEUE), Times.Once);
    }

    [Test]
    public async Task DeleteOfficeAsync_ValidOffice_ResultDelete()
    {
        // Arrange
        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeEntity.Id)).ReturnsAsync(officeEntity);
        _mapperMock.Setup(m => m.Map<OfficeDto>(officeEntity)).Returns(officeDto);

        // Act
        await _officeService.DeleteOfficeAsync(officeEntity.Id);

        // Assert
        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeEntity.Id), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.DeleteAsync(officeEntity.Id), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(officeEntity), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, RabbitMQQueues.DELETE_OFFICE_QUEUE), Times.Once);
    }
}
