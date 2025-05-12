using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Context;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Repositories;

/// <summary>
/// Repository class for handling operations related to office entities.
/// </summary>
public class OfficeRepository(MongoDbContext _context) : BaseRepository<OfficeEntity>(_context.OfficesCollection), IOfficeRepository
{
    /// <summary>
    /// Retrieves all OfficeEntity objects asynchronously.
    /// </summary>
    /// <returns>An enumerable collection of OfficeEntity objects.</returns>
    public override async Task<IEnumerable<OfficeEntity>> GetAllAsync()
    {
        return await _context.OfficesCollection
            .Find(entity => true)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all active offices asynchronously.
    /// </summary>
    /// <returns>A collection of active office entities.</returns>
    public async Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync()
    {
        return await _context.OfficesCollection.Find(office => office.IsActive == true).ToListAsync();
    }
}