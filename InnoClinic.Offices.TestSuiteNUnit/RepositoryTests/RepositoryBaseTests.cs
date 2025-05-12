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
            CollectionsNames = new List<string> { "Offices" }
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

        Assert.IsNotNull(result);
        Assert.AreEqual(office.Id, result.Id);
        Assert.AreEqual(office.City, result.City);
        Assert.AreEqual(office.Street, result.Street);
        Assert.AreEqual(office.HouseNumber, result.HouseNumber);
        Assert.AreEqual(office.Longitude, result.Longitude);
        Assert.AreEqual(office.Latitude, result.Latitude);
        Assert.AreEqual(office.RegistryPhoneNumber, result.RegistryPhoneNumber);
        Assert.AreEqual(office.IsActive, result.IsActive);
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

        Assert.IsNotNull(result);
        Assert.AreEqual("CityUpdate", result.City);
        Assert.AreEqual("StreetUpdate", result.Street);
        Assert.AreEqual("HouseNumberUpdate", result.HouseNumber);
        Assert.AreEqual("LongitudeUpdate", result.Longitude);
        Assert.AreEqual("LatitudeUpdate", result.Latitude);
        Assert.AreEqual("RegistryPhoneNumberUpdate", result.RegistryPhoneNumber);
        Assert.AreEqual(false, result.IsActive);
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

        Assert.IsNull(result);
    }
}
