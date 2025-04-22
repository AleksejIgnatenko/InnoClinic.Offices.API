using InnoClinic.Offices.Core.Models;
using MongoDB.Driver;

namespace InnoClinic.Offices.DataAccess.Repositories;

public abstract class RepositoryBase<T>(IMongoCollection<T> _collection) : IBaseRepository<T> where T : EntityBase
{
    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(e => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(e => e.Id == id);
    }
}