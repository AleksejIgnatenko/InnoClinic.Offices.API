namespace InnoClinic.Offices.Core.Models.OfficeModels;

public class OfficeEntity : EntityBase
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string OfficeNumber { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string? PhotoId { get; set; } = string.Empty;
    public string RegistryPhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}