using InnoClinic.Offices.Core.Models;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public interface IBaseRepository<T> where T : EntityBase
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}