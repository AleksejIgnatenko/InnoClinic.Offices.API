using InnoClinic.Offices.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Context;

public class CustomMongoClient
{
    protected readonly IMongoClient _mongoClient;

    public CustomMongoClient(IOptions<MongoOptions> options)
    {
        _mongoClient = new MongoClient(options.Value.ConnectionUri);
    }
}