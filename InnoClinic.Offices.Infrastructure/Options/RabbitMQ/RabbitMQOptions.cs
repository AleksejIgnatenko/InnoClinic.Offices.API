namespace InnoClinic.Offices.Infrastructure.Options.RabbitMQ;

/// <summary>
/// Represents the configuration options for connecting to RabbitMQ.
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// Gets or sets the host name of the RabbitMQ server.
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username for authenticating with RabbitMQ.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for authenticating with RabbitMQ.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}