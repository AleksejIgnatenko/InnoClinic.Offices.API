namespace InnoClinic.Offices.Core.Models.OfficeModels;

/// <summary>
/// Represents a data transfer object (DTO) for an office.
/// </summary>
public record OfficeDto(
    /// <summary>
    /// The unique identifier of the office.
    /// </summary>
    Guid Id,

    /// <summary>
    /// The city where the office is located.
    /// </summary>
    string City,

    /// <summary>
    /// The street where the office is located.
    /// </summary>
    string Street,

    /// <summary>
    /// The house number of the office.
    /// </summary>
    string HouseNumber,

    /// <summary>
    /// The office number (if applicable).
    /// </summary>
    string OfficeNumber,

    /// <summary>
    /// The registry phone number of the office.
    /// </summary>
    string RegistryPhoneNumber,

    /// <summary>
    /// Indicates whether the office is active.
    /// </summary>
    bool IsActive
);