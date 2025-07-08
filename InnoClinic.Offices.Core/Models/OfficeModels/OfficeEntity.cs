namespace InnoClinic.Offices.Core.Models.OfficeModels;

/// <summary>
/// Represents an entity that stores information about an office.
/// </summary>
public class OfficeEntity : EntityBase
{
    /// <summary>
    /// Gets or sets the city where the office is located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street where the office is located.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number of the office.
    /// </summary>
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the office number (if applicable).
    /// </summary>
    public string OfficeNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the longitude coordinate of the office location.
    /// </summary>
    public string Longitude { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the latitude coordinate of the office location.
    /// </summary>
    public string Latitude { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the photo associated with the office (if any).
    /// </summary>
    public string? PhotoId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registry phone number of the office.
    /// </summary>
    public string RegistryPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the office is active.
    /// </summary>
    public bool IsActive { get; set; }
}