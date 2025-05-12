using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Core.Abstractions;

/// <summary>
/// Represents a service for managing office entities.
/// </summary>
public interface IOfficeService
{
    /// <summary>
    /// Asynchronously creates a new office based on the provided office request.
    /// </summary>
    /// <param name="officeRequest">The request object containing office information.</param>
    /// <returns>The created OfficeEntity.</returns>
    Task<OfficeEntity> CreateOfficeAsync(OfficeRequest officeRequest);

    /// <summary>
    /// Asynchronously deletes an office by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the office to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteOfficeAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all offices.
    /// </summary>
    /// <returns>A collection of all OfficeEntity instances.</returns>
    Task<IEnumerable<OfficeEntity>> GetAllOfficesAsync();

    /// <summary>
    /// Asynchronously retrieves all active offices.
    /// </summary>
    /// <returns>A collection of active OfficeEntity instances.</returns>
    Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();

    /// <summary>
    /// Asynchronously retrieves an office by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the office to retrieve.</param>
    /// <returns>The OfficeEntity corresponding to the specified ID.</returns>
    Task<OfficeEntity> GetOfficeByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously updates an existing office with the provided data.
    /// </summary>
    /// <param name="id">The unique identifier of the office to update.</param>
    /// <param name="officeRequest">The request object containing updated office information.</param>
    /// <returns>The updated OfficeEntity.</returns>
    Task<OfficeEntity> UpdateOfficeAsync(Guid id, OfficeRequest officeRequest);
}