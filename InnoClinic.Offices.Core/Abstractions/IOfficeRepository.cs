using InnoClinic.Offices.Core.Models;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public interface IOfficeRepository
    {
        Task CreateAsync(OfficeModel office);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<OfficeModel>> GetAllAsync();
        Task<OfficeModel> GetByIdAsync(Guid id);
        Task UpdateAsync(OfficeModel office);
    }
}