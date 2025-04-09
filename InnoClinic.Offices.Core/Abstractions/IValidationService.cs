using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public interface IValidationService
    {
        Dictionary<string, string> Validation(OfficeEntity office);
    }
}