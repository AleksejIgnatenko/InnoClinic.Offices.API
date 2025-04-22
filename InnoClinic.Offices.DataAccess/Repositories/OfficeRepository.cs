using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Context;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Repositories;

public class OfficeRepository(MongoDbContext _context) : RepositoryBase<OfficeEntity>(_context.OfficesCollection), IOfficeRepository
{
    public async Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync()
    {
        return await _context.OfficesCollection.Find(o => o.IsActive == true).ToListAsync();
    }
}
