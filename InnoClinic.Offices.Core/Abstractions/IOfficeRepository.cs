using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Core.Abstractions;

/// <summary>
/// Represents a repository for managing office entities.
/// </summary>
public interface IOfficeRepository : IBaseRepository<OfficeEntity>
{
    /// <summary>
    /// Asynchronously retrieves all active offices.
    /// </summary>
    /// <returns>A collection of active OfficeEntity instances.</returns>
    Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();
}