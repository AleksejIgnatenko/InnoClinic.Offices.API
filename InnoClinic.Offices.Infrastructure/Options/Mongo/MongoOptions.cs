namespace InnoClinic.Offices.Infrastructure.Options.Mongo;

/// <summary>
/// Represents the options for configuring MongoDB connection and database settings.
/// </summary>
public class MongoOptions
{
    public string ConnectionUri { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public IEnumerable<string> CollectionsNames { get; set; } = [];
}
