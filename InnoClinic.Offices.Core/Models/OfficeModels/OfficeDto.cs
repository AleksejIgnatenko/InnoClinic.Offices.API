namespace InnoClinic.Offices.Core.Models.OfficeModels
{
    public record OfficeDto(
        Guid Id,
        string City,
        string Street,
        string HouseNumber,
        string OfficeNumber,
        string RegistryPhoneNumber,
        bool IsActive
    );
}