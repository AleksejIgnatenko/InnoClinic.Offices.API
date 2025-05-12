namespace InnoClinic.Offices.Core.Models.OfficeModels;

/// <summary>
/// Represents a request object for creating or updating an office.
/// </summary>
public record OfficeRequest(
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
    /// The ID of the photo associated with the office (if any).
    /// </summary>
    string? PhotoId,

    /// <summary>
    /// The registry phone number of the office.
    /// </summary>
    string RegistryPhoneNumber,

    /// <summary>
    /// Indicates whether the office is active.
    /// </summary>
    bool IsActive
);