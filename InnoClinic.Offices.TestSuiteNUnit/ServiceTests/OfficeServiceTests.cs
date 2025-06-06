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
    private Mock<IRabbitMQService> _rabbitMQServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IYandexGeocodingService> _yandexGeocodingServiceMock;

    private OfficeService _officeService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _officeRepositoryMock = _fixture.Freeze<Mock<IOfficeRepository>>();
        _rabbitMQServiceMock = _fixture.Freeze<Mock<IRabbitMQService>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _yandexGeocodingServiceMock = _fixture.Freeze<Mock<IYandexGeocodingService>>();

        _officeService = _fixture.Create<OfficeService>();
    }

    [Test]
    public async Task CreateOfficeAsync_ValidOffice_ResultCreated()
    {
        // Arrange
        var officeRequest = _fixture.Create<OfficeRequest>();
        var officeEntity = _fixture.Build<OfficeEntity>()
            .With(e => e.Id, Guid.NewGuid())
            .With(e => e.IsActive, true)
            .Create();
        var officeDto = _fixture.Create<OfficeDto>();

        _mapperMock.Setup(m => m.Map<OfficeEntity>(officeRequest)).Returns(officeEntity);
        _yandexGeocodingServiceMock.Setup(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("longitude", "latitude"));
        _officeRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<OfficeEntity>())).Returns(Task.FromResult(officeEntity));
        _mapperMock.Setup(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>())).Returns(officeDto);

        // Act
        await _officeService.CreateOfficeAsync(officeRequest);

        // Assert
        _mapperMock.Verify(m => m.Map<OfficeEntity>(officeRequest), Times.Once);
        _yandexGeocodingServiceMock.Verify(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>()), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, OfficeQueuesEnum.AddOffice.ToString()), Times.Once);
    }

    [Test]
    public async Task GetAllOfficesAsync_ShouldReturnAllOffices()
    {
        // Arrange
        var officeEntities = _fixture.CreateMany<OfficeEntity>(2).ToList();

        _officeRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(officeEntities);

        // Act
        var result = await _officeService.GetAllOfficesAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(officeEntities.Count);
        result.Should().BeEquivalentTo(officeEntities);

        _officeRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllActiveOfficesAsync_ShouldReturnAllActiveOffices()
    {
        // Arrange
        var activeOffice = _fixture.Build<OfficeEntity>().With(o => o.IsActive, true).Create();
        var inactiveOffice = _fixture.Build<OfficeEntity>().With(o => o.IsActive, false).Create();

        var allOfficesInDb = new List<OfficeEntity> { activeOffice, inactiveOffice };

        _officeRepositoryMock.Setup(repo => repo.GetByConditionAsync(
                o => o.IsActive == true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<OfficeEntity, bool>> predicate, CancellationToken ct) =>
            {
                return allOfficesInDb.Where(predicate.Compile()).ToList();
            });

        // Act
        var result = await _officeService.GetAllActiveOfficesAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().ContainSingle();
        result.First().Should().BeEquivalentTo(activeOffice);

        _officeRepositoryMock.Verify(repo => repo.GetByConditionAsync(
            o => o.IsActive == true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetOfficeByIdAsync_ShouldReturnOfficeById()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeEntity = _fixture.Build<OfficeEntity>().With(e => e.Id, officeId).Create();

        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);

        // Act
        var result = await _officeService.GetOfficeByIdAsync(officeId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(officeId);

        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateOfficeAsync_ValidOffice_ResultUpdate()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeRequest = _fixture.Create<OfficeRequest>();
        var officeEntity = _fixture.Build<OfficeEntity>()
            .With(e => e.Id, officeId)
            .Create();
        var officeDto = _fixture.Build<OfficeDto>()
            .With(d => d.Id, officeId)
            .Create();

        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);

        _mapperMock.Setup(m => m.Map(officeRequest, officeEntity)).Verifiable();
        _yandexGeocodingServiceMock.Setup(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("longitude", "latitude"));

        _officeRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<OfficeEntity>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>())).Returns(officeDto);

        // Act
        await _officeService.UpdateOfficeAsync(officeId, officeRequest, CancellationToken.None);

        // Assert
        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map(officeRequest, officeEntity), Times.Once);
        _yandexGeocodingServiceMock.Verify(service => service.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<OfficeEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(It.IsAny<OfficeEntity>()), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, OfficeQueuesEnum.UpdateOffice.ToString()), Times.Once);
    }

    [Test]
    public async Task DeleteOfficeAsync_ValidOffice_ResultDelete()
    {
        // Arrange
        var officeId = _fixture.Create<Guid>();
        var officeEntity = _fixture.Build<OfficeEntity>().With(e => e.Id, officeId).Create();
        var officeDto = _fixture.Build<OfficeDto>().With(d => d.Id, officeId).Create();

        _officeRepositoryMock.Setup(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(officeEntity);

        _mapperMock.Setup(m => m.Map<OfficeDto>(officeEntity)).Returns(officeDto);

        // Act
        await _officeService.DeleteOfficeAsync(officeId, CancellationToken.None);

        // Assert
        _officeRepositoryMock.Verify(repo => repo.GetByIdAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        _officeRepositoryMock.Verify(repo => repo.DeleteAsync(officeId), Times.Once);
        _mapperMock.Verify(m => m.Map<OfficeDto>(officeEntity), Times.Once);
        _rabbitMQServiceMock.Verify(service => service.PublishMessageAsync(officeDto, OfficeQueuesEnum.DeleteOffice.ToString()), Times.Once);
    }
}