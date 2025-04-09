using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly IMongoCollection<OfficeEntity> _collection;
        private const string COLLECTION_NAME = "Offices";

        public OfficeRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<OfficeEntity>(COLLECTION_NAME);
        }

        public async Task CreateAsync(OfficeEntity office)
        {
            await _collection.InsertOneAsync(office);
        }

        public async Task<IEnumerable<OfficeEntity>> GetAllAsync()
        {
            return await _collection.Find(o => true).ToListAsync();
        }

        public async Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync()
        {
            return await _collection.Find(o => o.IsActive == true).ToListAsync();
        }

        public async Task<OfficeEntity> GetByIdAsync(Guid id)
        {
            return await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(OfficeEntity office)
        {
            await _collection.ReplaceOneAsync(o => o.Id == office.Id, office);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(o => o.Id == id);
        }
    }
}
