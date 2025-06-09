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
public class OfficeRepositoryTests
{
    private MongoDbContainer _dbContainer;
    private MongoDbContext _context;
    private OfficeRepository _repository;

    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

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

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _context.OfficesCollection.Database.DropCollectionAsync("Offices");
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    public async Task GetByConditionAsync_WhenActiveOnly_ShouldReturnOnlyActiveOffices()
    {
        // Arrange
        var activeOffice = _fixture.Build<OfficeEntity>()
            .With(o => o.IsActive, true)
            .Create();

        var inactiveOffice = _fixture.Build<OfficeEntity>()
            .With(o => o.IsActive, false)
            .Create();

        await _repository.CreateAsync(activeOffice);
        await _repository.CreateAsync(inactiveOffice);

        // Act
        var result = await _repository.GetByConditionAsync(e => e.IsActive, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(e => e.Id == activeOffice.Id);
        result.Should().OnlyContain(e => e.IsActive);
    }
}