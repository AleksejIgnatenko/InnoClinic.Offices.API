using InnoClinic.Offices.Core.Models;
using InnoClinic.Offices.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly IMongoCollection<OfficeModel> _collection;
        private const string COLLECTION_NAME = "Offices";

        public OfficeRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<OfficeModel>(COLLECTION_NAME);
        }

        public async Task CreateAsync(OfficeModel office)
        {
            await _collection.InsertOneAsync(office);
        }

        public async Task<IEnumerable<OfficeModel>> GetAllAsync()
        {
            return await _collection.Find(o => true).ToListAsync();
        }

        public async Task<OfficeModel> GetByIdAsync(Guid id)
        {
            return await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(OfficeModel office)
        {
            await _collection.ReplaceOneAsync(o => o.Id == office.Id, office);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(o => o.Id == id);
        }
    }
}
