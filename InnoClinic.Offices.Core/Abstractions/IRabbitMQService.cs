namespace InnoClinic.Offices.Core.Abstractions;

/// <summary>
/// Represents a service for interacting with RabbitMQ messaging.
/// </summary>
public interface IRabbitMQService
{
    /// <summary>
    /// Asynchronously publishes a message to a specified queue.
    /// </summary>
    /// <param name="obj">The object to be published as a message.</param>
    /// <param name="queueName">The name of the queue to publish the message to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishMessageAsync(object obj, string queueName);

    /// <summary>
    /// Asynchronously creates necessary queues for RabbitMQ messaging.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateQueuesAsync();
}