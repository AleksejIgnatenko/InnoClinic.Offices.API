using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Options.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Context;

/// <summary>
/// Represents the MongoDB context for accessing collections related to office entities.
/// </summary>
public class MongoDbContext
{
    /// <summary>
    /// Gets or sets the MongoDB collection for offices.
    /// </summary>
    public IMongoCollection<OfficeEntity> OfficesCollection { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
    /// </summary>
    /// <param name="options">The MongoDB options.</param>
    /// <param name="client">The custom MongoDB client.</param>
    public MongoDbContext(IOptions<MongoOptions> options, CustomMongoClient client)
    {
        var database = client._mongoClient.GetDatabase(options.Value.DatabaseName);
        OfficesCollection = database.GetCollection<OfficeEntity>(options.Value.CollectionsNames.First());
    }
}