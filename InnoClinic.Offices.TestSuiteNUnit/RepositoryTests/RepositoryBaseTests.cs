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
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;

namespace InnoClinic.Offices.TestSuiteNUnit.RepositoryTests;

[TestFixture]
public class RepositoryBaseTests
{
    private MongoDbContainer _dbContainer;
    private MongoDbContext _context;
    private BaseRepository<OfficeEntity> _repository;

    private readonly Fixture _fixture = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        _fixture.Customize(new AutoMoqCustomization());
    }

    [SetUp]
    public async Task SetUp()
    {
        _dbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithName($"MongoTestContainer{Guid.NewGuid()}")
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
        await _context.OfficesCollection.DeleteManyAsync(_ => true);
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_ShouldAddEntity()
    {
        // Arrange
        var office = _fixture.Create<OfficeEntity>();

        // Act
        await _repository.CreateAsync(office);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(office);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var office = _fixture.Create<OfficeEntity>();
        await _repository.CreateAsync(office);

        var updatedOffice = _fixture.Build<OfficeEntity>()
            .With(e => e.Id, office.Id)
            .With(e => e.IsActive, false)
            .Create();

        // Act
        await _repository.UpdateAsync(updatedOffice);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedOffice, opt => opt.Excluding(o => o.Id));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var office = _fixture.Create<OfficeEntity>();
        await _repository.CreateAsync(office);

        // Act
        await _repository.DeleteAsync(office.Id);

        // Assert
        var result = await _context.OfficesCollection.Find(e => e.Id == office.Id).FirstOrDefaultAsync();
        result.Should().BeNull();
    }
}