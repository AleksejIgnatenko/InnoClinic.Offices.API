using InnoClinic.Offices.Infrastructure.Options.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Context;

/// <summary>
/// Represents a custom MongoDB client for interacting with the MongoDB database.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomMongoClient"/> class.
/// </remarks>
/// <param name="options">The MongoDB options used to configure the client.</param>
public class CustomMongoClient(IOptions<MongoOptions> options)
{
    /// <summary>
    /// The internal MongoDB client instance.
    /// </summary>
    internal readonly IMongoClient _mongoClient = new MongoClient(options.Value.ConnectionUri);
}