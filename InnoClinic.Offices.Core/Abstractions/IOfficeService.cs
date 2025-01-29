using InnoClinic.Offices.Core.Models;

namespace InnoClinic.Offices.Application.Services
{
    public interface IOfficeService
    {
        Task CreateOfficeAsync(string address, Guid photoId, string registryPhoneNumber, bool isActive);
        Task DeleteOfficeAsync(Guid id);
        Task<IEnumerable<OfficeModel>> GetAllOfficesAsync();
        Task<OfficeModel> GetOfficeByIdAsync(Guid id);
        Task UpdateOfficeAsync(string address, Guid photoId, string registryPhoneNumber, bool isActive);
    }
}