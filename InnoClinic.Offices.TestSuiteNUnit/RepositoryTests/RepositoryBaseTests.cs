using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Context;
using InnoClinic.Offices.DataAccess.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using InnoClinic.Offices.Infrastructure.Options.Mongo;

namespace InnoClinic.Offices.TestSuiteNUnit.RepositoryTests;

class RepositoryBaseTests
{
    private MongoDbContainer _dbContainer;
    private MongoDbContext _context;
    private BaseRepository<OfficeEntity> _repository;

    private OfficeEntity office;

    [SetUp]
    public async Task SetUp()
    {
        office = new OfficeEntity
        {
            Id = Guid.NewGuid(),
            City = "City",
            Street = "Street",
            HouseNumber = "HouseNumber",
            Longitude = "Longitude",
            Latitude = "Latitude",
            RegistryPhoneNumber = "RegistryPhoneNumber",
            IsActive = true,
        };

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        _dbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithName("MongoTestContainer")
            .WithUsername("mongoUser")
            .WithPassword("Password1!")
            .Build();

        await _dbContainer.StartAsync();

        var options = new MongoOptions
        {
            ConnectionUri = _dbContainer.GetConnectionString(),
            DatabaseName = "TestDatabase",
            CollectionsNames = ["Offices"]
        };

        var mongoClient = new CustomMongoClient(Options.Create(options));

        _context = new MongoDbContext(Options.Create(options), mongoClient);

        await _context.OfficesCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<OfficeEntity>(
                Builders<OfficeEntity>.IndexKeys.Ascending(x => x.Id),
                new CreateIndexOptions()
            ));

        _repository = new OfficeRepository(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_ShouldAddEntity()
    {
        // Act
        await _repository.CreateAsync(office);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(office.Id));
            Assert.That(result.City, Is.EqualTo(office.City));
            Assert.That(result.Street, Is.EqualTo(office.Street));
            Assert.That(result.HouseNumber, Is.EqualTo(office.HouseNumber));
            Assert.That(result.Longitude, Is.EqualTo(office.Longitude));
            Assert.That(result.Latitude, Is.EqualTo(office.Latitude));
            Assert.That(result.RegistryPhoneNumber, Is.EqualTo(office.RegistryPhoneNumber));
            Assert.That(result.IsActive, Is.EqualTo(office.IsActive));
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        await _repository.CreateAsync(office);

        // Act
        office.City = "CityUpdate";
        office.Street = "StreetUpdate";
        office.HouseNumber = "HouseNumberUpdate";
        office.Longitude = "LongitudeUpdate";
        office.Latitude = "LatitudeUpdate";
        office.RegistryPhoneNumber = "RegistryPhoneNumberUpdate";
        office.IsActive = false;

        await _repository.UpdateAsync(office);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.City, Is.EqualTo("CityUpdate"));
            Assert.That(result.Street, Is.EqualTo("StreetUpdate"));
            Assert.That(result.HouseNumber, Is.EqualTo("HouseNumberUpdate"));
            Assert.That(result.Longitude, Is.EqualTo("LongitudeUpdate"));
            Assert.That(result.Latitude, Is.EqualTo("LatitudeUpdate"));
            Assert.That(result.RegistryPhoneNumber, Is.EqualTo("RegistryPhoneNumberUpdate"));
            Assert.That(result.IsActive, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        await _repository.CreateAsync(office);

        // Act
        await _repository.DeleteAsync(office.Id);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();

        Assert.That(result, Is.Null);
    }
}
