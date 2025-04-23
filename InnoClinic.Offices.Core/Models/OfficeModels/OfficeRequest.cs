namespace InnoClinic.Offices.Core.Models.OfficeModels;

public record OfficeRequest(
    string City,
    string Street,
    string HouseNumber,
    string OfficeNumber,
    string? PhotoId,
    string RegistryPhoneNumber,
    bool IsActive
    );