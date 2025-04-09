using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public interface IOfficeRepository
    {
        Task CreateAsync(OfficeEntity office);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<OfficeEntity>> GetAllAsync();
        Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();
        Task<OfficeEntity> GetByIdAsync(Guid id);
        Task UpdateAsync(OfficeEntity office);
    }
}