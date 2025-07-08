using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.DataAccess.Context;

namespace InnoClinic.Offices.DataAccess.Repositories;

/// <summary>
/// Repository class for handling operations related to office entities.
/// </summary>
public class OfficeRepository(MongoDbContext _context) : BaseRepository<OfficeEntity>(_context.OfficesCollection), IOfficeRepository
{
}