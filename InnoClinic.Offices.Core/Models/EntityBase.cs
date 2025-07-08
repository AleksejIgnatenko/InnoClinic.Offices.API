namespace InnoClinic.Offices.Core.Models;

/// <summary>
/// Base class representing an entity with a unique identifier.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}