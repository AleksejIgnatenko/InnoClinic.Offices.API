using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public interface IOfficeService
    {
        Task<OfficeEntity> CreateOfficeAsync(OfficeRequest officeRequest);
        Task DeleteOfficeAsync(Guid id);
        Task<IEnumerable<OfficeEntity>> GetAllOfficesAsync();
        Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();
        Task<OfficeEntity> GetOfficeByIdAsync(Guid id);
        Task UpdateOfficeAsync(Guid id, OfficeRequest officeRequest);
    }
}