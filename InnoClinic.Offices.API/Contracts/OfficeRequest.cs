namespace InnoClinic.Offices.API.Contracts
{
    public record OfficeRequest(
        string Address,
        Guid PhotoId,
        string RegistryPhoneNumber,
        bool IsActive
        );
}
