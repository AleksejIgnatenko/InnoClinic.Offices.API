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

class OfficeRepositoryTests
{
    private MongoDbContainer _dbContainer;
    private MongoDbContext _context;
    private OfficeRepository _repository;

    private OfficeEntity office;
    private OfficeEntity office1;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
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

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _context.OfficesCollection.Database.DropCollectionAsync("Offices");
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

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

        office1 = new OfficeEntity
        {
            Id = Guid.NewGuid(),
            City = "City",
            Street = "Street",
            HouseNumber = "HouseNumber",
            Longitude = "Longitude",
            Latitude = "Latitude",
            RegistryPhoneNumber = "RegistryPhoneNumber",
            IsActive = false,
        };
    }

    [Test]
    public async Task GetAllActiveOfficesAsync_ShouldReturnsOnlyActiveOffices()
    {
        // Arrange
        await _repository.CreateAsync(office);
        await _repository.CreateAsync(office1);

        // Act
        var result = await _repository.GetAllActiveOfficesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Count());

        foreach (var office in result)
        {
            Assert.IsTrue(office.IsActive);
        }
    }
}
