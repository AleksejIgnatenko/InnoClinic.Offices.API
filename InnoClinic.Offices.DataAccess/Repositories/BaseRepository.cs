using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace InnoClinic.Offices.DataAccess.Repositories;

/// <summary>
/// Base repository class providing common CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The type of entity handled by the repository.</typeparam>
public abstract class BaseRepository<T>(IMongoCollection<T> collection) : IBaseRepository<T> where T : EntityBase
{
    private readonly IMongoCollection<T> _collection = collection;

    /// <summary>
    /// Creates a new entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    /// <summary>
    /// Retrieves all entities of type T asynchronously.
    /// </summary>
    /// <returns>An enumerable collection of entities of type T.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _collection
            .Find(entity => true)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves an entity of type T by its Id asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The entity with the specified Id.</returns>
    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _collection
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves all entities that match the specified condition.
    /// </summary>
    /// <param name="predicate">An expression that defines the condition for filtering the entities.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of entities that match the condition. 
    /// Returns an empty collection if no entities are found.
    /// </returns>
    public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _collection
            .Find(predicate)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Update entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    public async Task UpdateAsync(T entity)
    {
        await _collection.ReplaceOneAsync(entity => entity.Id.Equals(entity.Id), entity);
    }

    /// <summary>
    /// Delete entity of type T asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(entity => entity.Id == id);
    }
}