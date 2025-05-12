using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models;
using MongoDB.Driver;

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
    public abstract Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retrieves an entity of type T by its Id asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The entity with the specified Id.</returns>
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _collection.Find(entity => entity.Id == id).FirstOrDefaultAsync();
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