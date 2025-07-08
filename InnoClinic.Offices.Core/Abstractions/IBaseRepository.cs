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
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves all entities that match the specified condition.
    /// </summary>
    /// <param name="predicate">An expression that defines the condition for filtering the entities.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of entities that match the condition. 
    /// Returns an empty collection if no matching entities are found.
    /// </returns>
    Task<IEnumerable<T>> GetByConditionAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>The entity of type T corresponding to the specified ID.</returns>
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(T entity);
}