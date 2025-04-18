using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.DataAccess.Repositories
{
    public interface IOfficeRepository : IBaseRepository<OfficeEntity>
    {
        Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();
    }
}