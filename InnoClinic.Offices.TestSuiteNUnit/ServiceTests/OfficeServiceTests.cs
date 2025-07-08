using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Enums.Queues;
using Moq;
using System.Linq.Expressions;
using FluentAssertions;

namespace InnoClinic.Offices.TestSuiteNUnit.ServiceTests;

[TestFixture]
public class OfficeServiceTests
{
    private IFixture _fixture;
    private Mock<IOfficeRepository> _officeRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IYandexGeocodingService> _yandexGeocodingServiceMock;
    private Mock<IRabbitMQService> _rabbitMQServiceMock;

    private OfficeService _officeService;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _officeRepositoryMock = _fixture.Freeze<Mock<IOfficeRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _yandexGeocodingServiceMock = _fixture.Freeze<Mock<IYandexGeocodingService>>();
        _rabbitMQServiceMock = _fixture.Freeze<Mock<IRabbitMQService>>();

        _officeService = _fixture.Create<OfficeService>();
    }

    [Test]
    public async Task CreateOfficeAsync_ValidOffice_ResultCreated()
    {
        // Arrange
        var officeRequest = _fixture.Create<OfficeRequest>();
        var officeEntity = _fixture.Build<OfficeEntity>()
            .With(e => e.IsActive, true)
            .Create();
        var officeDto = _fixture.Create<OfficeDto>();

        _mapperMock.Setup(m => m.Map<OfficeEntity>(officeRequest)).Returns(officeEntity);
        _yandexGeocodingServiceMock.Setup(s => s.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("longitude", "latitude"));
        _officeRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<OfficeEntity>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>())).Returns(officeDto);

        // Act
        await _officeService.CreateOfficeAsync(officeRequest);

        // Assert
        _mapperMock.Verify(m => m.Map<OfficeEntity>(officeRequest), Times.Once);
        _yandexGeocodingServiceMock.Verify(s => s.GetCoordinatesAsync(
            officeEntity.City,
            officeEntity.Street,
            officeEntity.HouseNumber), Times.Once);
        _officeRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>()), Times.Once);
        _rabbitMQServiceMock.Verify(r => r.PublishMessageAsync(officeDto, OfficeQueuesEnum.AddOffice.ToString()), Times.Once);
    }

    [Test]
    public async Task GetAllOfficesAsync_ShouldReturnAllOffices()
    {
        // Arrange
        var offices = _fixture.CreateMany<OfficeEntity>(3).ToList();

        _officeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(offices);

        // Act
        var result = await _officeService.GetAllOfficesAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(offices);
        _officeRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllActiveOfficesAsync_ShouldReturnOnlyActiveOffices()
    {
        // Arrange
        var activeOffice = _fixture.Build<OfficeEntity>().With(o => o.IsActive, true).Create();
        var inactiveOffice = _fixture.Build<OfficeEntity>().With(o => o.IsActive, false).Create();

        var allOffices = new List<OfficeEntity> { activeOffice, inactiveOffice };

        _officeRepositoryMock.Setup(r => r.GetByConditionAsync(
                It.IsAny<Expression<Func<OfficeEntity, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<OfficeEntity, bool>> predicate, CancellationToken ct) =>
            {
                return allOffices.Where(predicate.Compile()).ToList();
            });

        // Act
        var result = await _officeService.GetAllActiveOfficesAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(o => o.Id == activeOffice.Id);
        result.Should().OnlyContain(o => o.IsActive);
    }

    [Test]
    public async Task GetOfficeByIdAsync_ShouldReturnOffice()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeEntity = _fixture.Build<OfficeEntity>().With(e => e.Id, officeId).Create();

        _officeRepositoryMock.Setup(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);

        // Act
        var result = await _officeService.GetOfficeByIdAsync(officeId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(officeEntity);
        _officeRepositoryMock.Verify(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateOfficeAsync_ValidOffice_UpdatesAndPublishes()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeEntity = _fixture.Build<OfficeEntity>().With(e => e.Id, officeId).Create();
        var officeRequest = _fixture.Create<OfficeRequest>();
        var officeDto = _fixture.Build<OfficeDto>().With(d => d.Id, officeId).Create();

        _officeRepositoryMock.Setup(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);

        _mapperMock.Setup(m => m.Map(officeRequest, officeEntity)).Callback(() =>
        {
            officeEntity.City = officeRequest.City;
            officeEntity.Street = officeRequest.Street;
            officeEntity.HouseNumber = officeRequest.HouseNumber;
        });

        _yandexGeocodingServiceMock.Setup(s => s.GetCoordinatesAsync(
                officeRequest.City, officeRequest.Street, officeRequest.HouseNumber))
            .ReturnsAsync(("12.345", "67.890"));

        _officeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<OfficeEntity>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>())).Returns(officeDto);

        // Act
        await _officeService.UpdateOfficeAsync(officeId, officeRequest, CancellationToken.None);

        // Assert
        _officeRepositoryMock.Verify(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map(officeRequest, officeEntity), Times.Once);
        _yandexGeocodingServiceMock.Verify(s => s.GetCoordinatesAsync(
            officeRequest.City, officeRequest.Street, officeRequest.HouseNumber), Times.Once);
        _officeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>()), Times.Once);
        _rabbitMQServiceMock.Verify(r => r.PublishMessageAsync(officeDto, OfficeQueuesEnum.UpdateOffice.ToString()), Times.Once);
    }

    [Test]
    public async Task DeleteOfficeAsync_ValidOffice_DeletesAndPublishes()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeEntity = _fixture.Build<OfficeEntity>().With(e => e.Id, officeId).Create();
        var officeDto = _fixture.Build<OfficeDto>().With(d => d.Id, officeId).Create();

        _officeRepositoryMock.Setup(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);
        _mapperMock.Setup(m => m.Map<OfficeDto>(officeEntity)).Returns(officeDto);

        // Act
        await _officeService.DeleteOfficeAsync(officeId, CancellationToken.None);

        // Assert
        _officeRepositoryMock.Verify(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        _officeRepositoryMock.Verify(r => r.DeleteAsync(officeId), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(officeEntity), Times.Once);
        _rabbitMQServiceMock.Verify(r => r.PublishMessageAsync(officeDto, OfficeQueuesEnum.DeleteOffice.ToString()), Times.Once);
    }
}