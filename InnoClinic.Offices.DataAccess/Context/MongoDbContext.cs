using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Context;

public class MongoDbContext
{
    public IMongoCollection<OfficeEntity> OfficesCollection { get; set; }

    public MongoDbContext(IOptions<MongoOptions> options, IMongoClient client)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        OfficesCollection = database.GetCollection<OfficeEntity>(options.Value.CollectionsNames.First());
    }
}
