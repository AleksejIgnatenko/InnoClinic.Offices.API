using InnoClinic.Offices.Core.Models;

namespace InnoClinic.Offices.Core.Abstractions;

/// <summary>
/// Represents a base repository interface for CRUD operations on entities of type T.
/// </summary>
/// <typeparam name="T">The type of entity that the repository works with.</typeparam>
public interface IBaseRepository<T> where T : EntityBase
{
    /// <summary>
    /// Asynchronously creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    Task CreateAsync(T entity);

    /// <summary>
    /// Asynchronously deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all entities.
    /// </summary>
    /// <returns>A collection of entities of type T.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>The entity of type T corresponding to the specified ID.</returns>
    Task<T> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(T entity);
}