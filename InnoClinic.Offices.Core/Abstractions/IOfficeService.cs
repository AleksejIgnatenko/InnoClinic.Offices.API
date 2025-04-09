using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public interface IOfficeService
    {
        Task CreateOfficeAsync(string city, string street, string houseNumber, string officeNumber, string? photoId, string registryPhoneNumber, bool isActive);
        Task DeleteOfficeAsync(Guid id);
        Task<IEnumerable<OfficeEntity>> GetAllOfficesAsync();
        Task<IEnumerable<OfficeEntity>> GetAllActiveOfficesAsync();
        Task<OfficeEntity> GetOfficeByIdAsync(Guid id);
        Task UpdateOfficeAsync(Guid id, string city, string street, string houseNumber, string officeNumber, string? photoId, string registryPhoneNumber, bool isActive);
    }
}